using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using NovaScale.Platform.API.ConversationManagement.Application.Services;
using NovaScale.Platform.API.ConversationManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.ConversationManagement.Domain.Model.Commands;
using NovaScale.Platform.API.ConversationManagement.Domain.Model.ValueObjects;
using NovaScale.Platform.API.IdentityAccess.Domain.Model.Aggregates;
using NovaScale.Platform.API.LeadManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.Shared.Application.Security;
using NovaScale.Platform.API.Shared.Application.Services;
using NovaScale.Platform.API.Shared.Domain.Exceptions;
using NovaScale.Platform.API.Shared.Domain.Model;
using NovaScale.Platform.API.Shared.Domain.Repositories;
using NovaScale.Platform.API.Shared.Infrastructure.Persistence.EFC.Configuration;

namespace NovaScale.Platform.API.ConversationManagement.Infrastructure.Gmail;

public sealed class GmailIntegrationService(AppDbContext db, IRequestContext request, IAccessPolicy access,
    IUnitOfWork unitOfWork, IEntityLifecycle<Message> lifecycle, GmailClient client, GmailOptions options,
    GmailOperationLocks locks, GmailAuthorizationStore authorizations, IDataProtectionProvider protection,
    IHttpContextAccessor httpContext, ILogger<GmailIntegrationService> logger) : IGmailIntegrationService
{
    private const string CookieName = "novaleads.gmail.oauth";
    private readonly IDataProtector protector = protection.CreateProtector("NovaLeads.GmailTokens.v1");

    public async Task<GmailStatus> StatusAsync(CancellationToken cancellationToken)
    {
        await access.RequireAsync("read_conversations", cancellationToken);
        var account = await db.Set<GmailAccount>().AsNoTracking().SingleOrDefaultAsync(cancellationToken);
        return new GmailStatus(options.Configured, account is not null, account?.Email, await access.IsOwnerAsync(cancellationToken));
    }

    public async Task<string> ConnectAsync(CancellationToken cancellationToken)
    {
        await access.RequireAsync("manage_integrations", cancellationToken);
        if (!options.Configured) throw new BusinessException("GmailNotConfigured", 503);
        var state = GmailMimeCodec.Encode(RandomNumberGenerator.GetBytes(48));
        var verifier = GmailMimeCodec.Encode(RandomNumberGenerator.GetBytes(48));
        authorizations.Add(state, new GmailAuthorization(request.OrganizationId, request.UserId, verifier, DateTime.UtcNow.AddMinutes(10)));
        var secureCookie = options.CallbackUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase);
        httpContext.HttpContext!.Response.Cookies.Append(CookieName, state, new CookieOptions
        {
            HttpOnly = true, Secure = secureCookie,
            SameSite = secureCookie ? SameSiteMode.None : SameSiteMode.Lax,
            MaxAge = TimeSpan.FromMinutes(10), Path = "/api/v1/integrations/gmail"
        });
        return QueryHelpers.AddQueryString("https://accounts.google.com/o/oauth2/v2/auth", new Dictionary<string, string?>
        {
            ["client_id"] = options.ClientId, ["redirect_uri"] = options.CallbackUrl,
            ["response_type"] = "code", ["scope"] = GmailOptions.SendScope + " " + GmailOptions.ReadScope,
            ["state"] = state, ["access_type"] = "offline", ["prompt"] = "consent",
            ["code_challenge"] = GmailMimeCodec.Encode(SHA256.HashData(Encoding.ASCII.GetBytes(verifier))),
            ["code_challenge_method"] = "S256"
        });
    }

    public async Task CompleteConnectionAsync(string? code, string? state, CancellationToken cancellationToken)
    {
        var cookie = httpContext.HttpContext!.Request.Cookies[CookieName];
        httpContext.HttpContext.Response.Cookies.Delete(CookieName, new CookieOptions { Path = "/api/v1/integrations/gmail" });
        if (string.IsNullOrWhiteSpace(state) || string.IsNullOrWhiteSpace(cookie)
            || !CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(state), Encoding.UTF8.GetBytes(cookie)))
            throw new BusinessException("GmailAuthorizationFailed", 400);
        var pending = authorizations.Consume(state) ?? throw new BusinessException("GmailAuthorizationFailed", 400);
        if (string.IsNullOrWhiteSpace(code)) throw new BusinessException("GmailAuthorizationFailed", 400);
        var stillOwner = await (from user in db.Set<User>().IgnoreQueryFilters()
            join role in db.Set<Role>().IgnoreQueryFilters() on new { user.OrganizationId, Id = user.RoleId } equals new { role.OrganizationId, role.Id }
            where user.Id == pending.UserId && user.OrganizationId == pending.OrganizationId && user.Status == "ACTIVE" && role.SystemCode == "OWNER"
            select user.Id).AnyAsync(cancellationToken);
        if (!stillOwner) throw new BusinessException("Forbidden", 403);
        var gate = locks.ForOrganization(pending.OrganizationId);
        await gate.WaitAsync(cancellationToken);
        try
        {
            var tokens = await client.ExchangeAsync(new Dictionary<string, string>
            {
                ["code"] = code, ["redirect_uri"] = options.CallbackUrl, ["grant_type"] = "authorization_code", ["code_verifier"] = pending.Verifier
            }, cancellationToken);
            var scopes = tokens.TryGetProperty("scope", out var scope) ? scope.GetString()?.Split(' ') ?? [] : [];
            if (!scopes.Contains(GmailOptions.ReadScope) || !scopes.Contains(GmailOptions.SendScope))
                throw new BusinessException("GmailPermissionDenied", 403);
            var token = tokens.GetProperty("access_token").GetString()!;
            var profile = await client.GetAsync(token, "profile", cancellationToken);
            var email = Guard.Email(profile.GetProperty("emailAddress").GetString()!);
            var account = await db.Set<GmailAccount>().IgnoreQueryFilters()
                .SingleOrDefaultAsync(item => item.OrganizationId == pending.OrganizationId, cancellationToken);
            var refresh = tokens.TryGetProperty("refresh_token", out var refreshValue) ? refreshValue.GetString() : null;
            if (string.IsNullOrWhiteSpace(refresh)) throw new BusinessException("GmailReconnect", 409);
            if (account is null)
            {
                account = new GmailAccount { OrganizationId = pending.OrganizationId };
                db.Add(account);
            }
            account.ConnectedByUserId = pending.UserId;
            account.Email = email;
            account.ProtectedRefreshToken = protector.Protect(refresh);
            account.ProtectedAccessToken = protector.Protect(token);
            account.AccessTokenExpiresAt = DateTime.UtcNow.AddSeconds(tokens.GetProperty("expires_in").GetInt32() - 60);
            await db.SaveChangesAsync(cancellationToken);
        }
        finally { gate.Release(); }
    }

    public string ReturnUrl(bool connected) => QueryHelpers.AddQueryString(options.FrontendUrl, "gmail", connected ? "connected" : "error");

    public async Task DisconnectAsync(CancellationToken cancellationToken)
    {
        await access.RequireAsync("manage_integrations", cancellationToken);
        var gate = locks.ForOrganization(request.OrganizationId);
        await gate.WaitAsync(cancellationToken);
        try
        {
            var account = await db.Set<GmailAccount>().SingleOrDefaultAsync(cancellationToken);
            if (account is null) return;
            db.Remove(account);
            await db.SaveChangesAsync(cancellationToken);
        }
        finally { gate.Release(); }
    }

    private async Task<Contact> RequireContactAsync(long contactId, CancellationToken cancellationToken)
    {
        await access.RequireAsync("write_conversations", cancellationToken);
        var contact = await db.Set<Contact>().SingleOrDefaultAsync(item => item.Id == contactId, cancellationToken)
            ?? throw new BusinessException("NotFound", 404);
        if (contact.UserId != request.UserId && !await access.IsOwnerAsync(cancellationToken)) throw new BusinessException("Forbidden", 403);
        if (string.IsNullOrWhiteSpace(contact.Email)) throw new BusinessException("GmailContactEmailRequired");
        return contact;
    }

    private async Task<(GmailAccount Account, string Token)> AccountAsync(CancellationToken cancellationToken)
    {
        var account = await db.Set<GmailAccount>().SingleOrDefaultAsync(cancellationToken)
            ?? throw new BusinessException("GmailNotConnected", 409);
        try
        {
            if (account.AccessTokenExpiresAt > DateTime.UtcNow) return (account, protector.Unprotect(account.ProtectedAccessToken));
            var tokens = await client.ExchangeAsync(new Dictionary<string, string>
            {
                ["refresh_token"] = protector.Unprotect(account.ProtectedRefreshToken), ["grant_type"] = "refresh_token"
            }, cancellationToken);
            var token = tokens.GetProperty("access_token").GetString()!;
            account.ProtectedAccessToken = protector.Protect(token);
            account.AccessTokenExpiresAt = DateTime.UtcNow.AddSeconds(tokens.GetProperty("expires_in").GetInt32() - 60);
            await db.SaveChangesAsync(cancellationToken);
            return (account, token);
        }
        catch (CryptographicException) { throw new BusinessException("GmailReconnect", 409); }
    }

    public async Task<Message> SendAsync(long contactId, GmailSendRequest resource, CancellationToken cancellationToken)
    {
        var contact = await RequireContactAsync(contactId, cancellationToken);
        if (resource.RequestId == Guid.Empty) throw new BusinessException("InvalidData");
        var content = Guard.Text(resource.Content, 16000, false);
        var files = resource.Attachments ?? [];
        if (files.Count > 3 || files.Any(file => file is null) || (content.Length == 0 && files.Count == 0)) throw new BusinessException("InvalidData");
        foreach (var file in files) file.Validate();
        var gate = locks.ForOrganization(request.OrganizationId);
        await gate.WaitAsync(cancellationToken);
        try
        {
            var (account, token) = await AccountAsync(cancellationToken);
            if (account.Email.Equals(contact.Email, StringComparison.OrdinalIgnoreCase)) throw new BusinessException("GmailSameRecipient");
            var prior = await db.Set<Message>().SingleOrDefaultAsync(item => item.ClientRequestId == resource.RequestId, cancellationToken);
            if (prior is not null)
            {
                var belongsToContact = await db.Set<Conversation>().AnyAsync(item => item.Id == prior.ConversationId && item.ContactId == contactId, cancellationToken);
                if (!belongsToContact || prior.UserId != contact.UserId) throw new BusinessException("Forbidden", 403);
                if (prior.Content != content) throw new BusinessException("InvalidData");
                if (prior.DeliveryStatus == "SENT") return prior;
                throw new BusinessException("GmailDeliveryUncertain", 409);
            }
            var conversation = await db.Set<Conversation>().Where(item => item.ContactId == contactId && item.UserId == contact.UserId
                && item.Channel == "GMAIL" && item.GmailMailbox == account.Email && item.GmailRecipientEmail == contact.Email && item.Status == "OPEN")
                .OrderByDescending(item => item.LastMessageAt).FirstOrDefaultAsync(cancellationToken);
            MimeMessage? parent = null;
            if (conversation?.GmailThreadId is { } threadId)
            {
                var thread = await client.GetAsync(token, "threads/" + Uri.EscapeDataString(threadId) + "?format=metadata&metadataHeaders=Subject&metadataHeaders=Message-ID&metadataHeaders=References", cancellationToken);
                var last = thread.GetProperty("messages").EnumerateArray().LastOrDefault();
                if (last.ValueKind != JsonValueKind.Undefined)
                {
                    parent = new MimeMessage();
                    foreach (var header in last.GetProperty("payload").GetProperty("headers").EnumerateArray())
                    {
                        var name = header.GetProperty("name").GetString();
                        var value = header.GetProperty("value").GetString();
                        if (name?.Equals("Message-ID", StringComparison.OrdinalIgnoreCase) == true && !string.IsNullOrWhiteSpace(value)) parent.MessageId = value.Trim('<', '>');
                        if (name?.Equals("Subject", StringComparison.OrdinalIgnoreCase) == true) parent.Subject = value ?? "";
                    }
                }
            }
            var subject = parent?.Subject ?? conversation?.GmailSubject ?? "NovaLeads - " + contact.FullName;
            if (subject.Length > 255) subject = subject[..255];
            if (conversation is null)
            {
                conversation = new Conversation(new CreateConversationCommand("GMAIL", "OPEN", contact.Id, contact.UserId)) { OrganizationId = request.OrganizationId };
                conversation.LinkGmail(account.Email, null, subject, contact.Email);
            }
            var messageId = resource.RequestId.ToString("N") + "." + request.OrganizationId + "@novaleads.local";
            var raw = await GmailMimeCodec.ComposeAsync(account.Email, contact.Email, subject, messageId, content, files, parent, cancellationToken);
            parent?.Dispose();
            var message = await unitOfWork.ExecuteAsync(async () =>
            {
                if (conversation.Id == 0) { db.Add(conversation); await unitOfWork.CompleteAsync(cancellationToken); }
                var entity = new Message(new CreateMessageCommand("OUT", content, conversation.Id)) { OrganizationId = request.OrganizationId };
                entity.SetAttachments(CreateAttachments(files, entity.Id));
                entity.PrepareEmail(resource.RequestId, messageId);
                await lifecycle.BeforeSaveAsync(entity, true, cancellationToken);
                db.Add(entity);
                await unitOfWork.CompleteAsync(cancellationToken);
                return entity;
            }, cancellationToken);
            JsonElement sent;
            try
            {
                sent = await client.SendAsync(token, raw, conversation.GmailThreadId, cancellationToken);
            }
            catch (Exception exception)
            {
                if (exception is BusinessException { StatusCode: >= 400 and < 500 }) message.MarkDeliveryFailed();
                else message.MarkDeliveryUncertain();
                await db.SaveChangesAsync(CancellationToken.None);
                logger.LogWarning("Gmail send was not confirmed for message {MessageId}; status {Status}.", message.Id, message.DeliveryStatus);
                throw exception is BusinessException business ? business : new BusinessException("GmailDeliveryUncertain", 409);
            }
            conversation.LinkGmail(account.Email, sent.GetProperty("threadId").GetString(), subject, contact.Email);
            message.ApplyEmail(GmailMimeCodec.ExternalId(account.Email, sent.GetProperty("id").GetString()!), messageId, DateTime.UtcNow);
            await db.SaveChangesAsync(CancellationToken.None);
            await lifecycle.AfterSaveAsync(message, true, CancellationToken.None);
            await db.SaveChangesAsync(CancellationToken.None);
            return message;
        }
        finally { gate.Release(); }
    }

    public async Task<int> SyncAsync(long contactId, CancellationToken cancellationToken)
    {
        var contact = await RequireContactAsync(contactId, cancellationToken);
        var gate = locks.ForOrganization(request.OrganizationId);
        await gate.WaitAsync(cancellationToken);
        try
        {
            var (account, token) = await AccountAsync(cancellationToken);
            var query = "{from:" + contact.Email + " to:" + contact.Email + "}";
            var listing = await client.GetAsync(token, "messages?maxResults=30&q=" + Uri.EscapeDataString(query), cancellationToken);
            var knownThreads = await db.Set<Conversation>().AsNoTracking()
                .Where(item => item.ContactId == contactId && item.GmailMailbox == account.Email
                    && item.GmailRecipientEmail == contact.Email && item.Status == "OPEN" && item.GmailThreadId != null)
                .OrderByDescending(item => item.Id).Select(item => item.GmailThreadId!).Take(30).ToArrayAsync(cancellationToken);
            var listedThreads = listing.TryGetProperty("messages", out var entries)
                ? entries.EnumerateArray().Select(item => item.GetProperty("threadId").GetString()!).ToArray() : [];
            var threadIds = knownThreads.Concat(listedThreads).Distinct().ToArray();
            var imported = 0;
            foreach (var threadId in threadIds)
            {
                var thread = await client.GetAsync(token, "threads/" + Uri.EscapeDataString(threadId) + "?format=minimal", cancellationToken);
                foreach (var item in thread.GetProperty("messages").EnumerateArray())
                {
                    var providerId = item.GetProperty("id").GetString()!;
                    var externalId = GmailMimeCodec.ExternalId(account.Email, providerId);
                    if (await db.Set<Message>().AnyAsync(message => message.ExternalMessageId == externalId, cancellationToken)) continue;
                    var raw = await client.GetAsync(token, "messages/" + Uri.EscapeDataString(providerId) + "?format=raw", cancellationToken);
                    using var stream = new MemoryStream(GmailMimeCodec.Decode(raw.GetProperty("raw").GetString()!));
                    using var mime = await MimeMessage.LoadAsync(stream, cancellationToken);
                    var sender = mime.From.Mailboxes.FirstOrDefault()?.Address;
                    var outgoing = sender?.Equals(account.Email, StringComparison.OrdinalIgnoreCase) == true;
                    var expectedSender = outgoing ? account.Email : contact.Email;
                    var recipient = outgoing ? contact.Email : account.Email;
                    if (!string.Equals(sender, expectedSender, StringComparison.OrdinalIgnoreCase)
                        || !mime.To.Mailboxes.Concat(mime.Cc.Mailboxes).Any(address => address.Address.Equals(recipient, StringComparison.OrdinalIgnoreCase))) continue;
                    if (outgoing && !(raw.TryGetProperty("labelIds", out var labels) && labels.EnumerateArray().Any(label => label.GetString() == "SENT"))) continue;
                    var sentAt = raw.TryGetProperty("internalDate", out var internalDate) && long.TryParse(internalDate.GetString(), out var milliseconds)
                        ? DateTimeOffset.FromUnixTimeMilliseconds(milliseconds).UtcDateTime : mime.Date.UtcDateTime;
                    var pending = !outgoing || string.IsNullOrEmpty(mime.MessageId) ? null : await db.Set<Message>().FirstOrDefaultAsync(message => message.EmailMessageId == mime.MessageId && message.SenderType == "OUT", cancellationToken);
                    if (pending is not null)
                    {
                        var pendingConversation = await db.Set<Conversation>().SingleAsync(c => c.Id == pending.ConversationId, cancellationToken);
                        if (pendingConversation.ContactId != contactId || pendingConversation.GmailMailbox != account.Email) continue;
                        pending.ApplyEmail(externalId, mime.MessageId, sentAt);
                        var recoveredSubject = mime.Subject ?? "NovaLeads";
                        pendingConversation.LinkGmail(account.Email, threadId, recoveredSubject.Length > 255 ? recoveredSubject[..255] : recoveredSubject, contact.Email);
                        await db.SaveChangesAsync(cancellationToken);
                        await lifecycle.AfterSaveAsync(pending, false, cancellationToken);
                        await db.SaveChangesAsync(cancellationToken);
                        imported++;
                        continue;
                    }
                    var text = GmailMimeCodec.Text(mime);
                    var files = GmailMimeCodec.Attachments(mime);
                    await unitOfWork.ExecuteAsync(async () =>
                    {
                        if (await db.Set<Message>().AnyAsync(message => message.ExternalMessageId == externalId, cancellationToken)) return false;
                        var conversation = await db.Set<Conversation>().SingleOrDefaultAsync(c => c.GmailMailbox == account.Email && c.GmailThreadId == threadId && c.ContactId == contactId, cancellationToken);
                        if (conversation is null)
                        {
                            conversation = new Conversation(new CreateConversationCommand("GMAIL", "OPEN", contact.Id, contact.UserId)) { OrganizationId = request.OrganizationId };
                            var importedSubject = mime.Subject ?? "NovaLeads";
                            conversation.LinkGmail(account.Email, threadId, importedSubject.Length > 255 ? importedSubject[..255] : importedSubject, contact.Email);
                            db.Add(conversation);
                            await unitOfWork.CompleteAsync(cancellationToken);
                        }
                        if (conversation.Status != "OPEN") return false;
                        var message = new Message(new CreateMessageCommand(outgoing ? "OUT" : "IN", text, conversation.Id)) { OrganizationId = request.OrganizationId };
                        message.ApplyEmail(externalId, mime.MessageId, sentAt);
                        message.SetAttachments(CreateAttachments(files, 0));
                        await lifecycle.BeforeSaveAsync(message, true, cancellationToken);
                        db.Add(message);
                        await unitOfWork.CompleteAsync(cancellationToken);
                        await lifecycle.AfterSaveAsync(message, true, cancellationToken);
                        imported++;
                        return true;
                    }, cancellationToken);
                }
            }
            return imported;
        }
        finally { gate.Release(); }
    }

    private IReadOnlyList<Attachment> CreateAttachments(IReadOnlyList<MessageAttachment> files, long messageId) =>
        files.Select(file => new Attachment(new CreateAttachmentCommand(file.Name, file.ContentType, file.Data, messageId))
            { OrganizationId = request.OrganizationId }).ToArray();
}
