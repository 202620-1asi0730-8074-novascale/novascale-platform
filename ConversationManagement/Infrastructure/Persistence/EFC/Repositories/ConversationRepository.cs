using NovaScale.Platform.API.ConversationManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.ConversationManagement.Domain.Repositories;
using NovaScale.Platform.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using NovaScale.Platform.API.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace NovaScale.Platform.API.ConversationManagement.Infrastructure.Persistence.EFC.Repositories;

public class ConversationRepository(AppDbContext context) : BaseRepository<Conversation>(context), IConversationRepository { }
