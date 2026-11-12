using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NovaScale.Platform.API.Shared.Infrastructure.Persistence.EFC.Configuration;

#nullable disable

namespace NovaScale.Platform.API.Shared.Infrastructure.Persistence.EFC.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20261102233248_FixDateOnlyMapping")]
    partial class FixDateOnlyMapping
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "10.0.13")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("NovaScale.Platform.API.ConversationManagement.Domain.Model.Aggregates.Conversation", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id_conversation");

                    b.Property<string>("Channel")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("varchar(30)")
                        .HasColumnName("channel");

                    b.Property<long>("ContactId")
                        .HasColumnType("bigint")
                        .HasColumnName("id_contact");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("created_at");

                    b.Property<DateTime?>("LastMessageAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("last_message_at");

                    b.Property<long>("OrganizationId")
                        .HasColumnType("bigint")
                        .HasColumnName("id_organization");

                    b.Property<DateTime?>("PendingSince")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("pending_since");

                    b.Property<string>("Priority")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("varchar(10)")
                        .HasColumnName("priority");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("varchar(30)")
                        .HasColumnName("status");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("updated_at");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint")
                        .HasColumnName("id_user");

                    b.HasKey("Id");

                    b.HasIndex("OrganizationId", "ContactId");

                    b.HasIndex("OrganizationId", "UserId");

                    b.ToTable("conversations", (string)null);
                });

            modelBuilder.Entity("NovaScale.Platform.API.ConversationManagement.Domain.Model.Aggregates.Message", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id_message");

                    b.Property<string>("AttachmentsJson")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("attachments_json");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasMaxLength(16000)
                        .HasColumnType("text")
                        .HasColumnName("content");

                    b.Property<long>("ConversationId")
                        .HasColumnType("bigint")
                        .HasColumnName("id_conversation");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("created_at");

                    b.Property<string>("ExternalMessageId")
                        .HasMaxLength(120)
                        .HasColumnType("varchar(120)")
                        .HasColumnName("external_message_id");

                    b.Property<long>("OrganizationId")
                        .HasColumnType("bigint")
                        .HasColumnName("id_organization");

                    b.Property<string>("SenderType")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("varchar(30)")
                        .HasColumnName("sender_type");

                    b.Property<DateTime>("SentAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("sent_at");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("updated_at");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint")
                        .HasColumnName("id_user");

                    b.HasKey("Id");

                    b.HasAlternateKey("OrganizationId", "Id");

                    b.HasIndex("OrganizationId", "ConversationId");

                    b.HasIndex("OrganizationId", "ExternalMessageId")
                        .IsUnique();

                    b.HasIndex("OrganizationId", "UserId");

                    b.ToTable("messages", (string)null);
                });

            modelBuilder.Entity("NovaScale.Platform.API.ConversationManagement.Domain.Model.Aggregates.Notification", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id_notification");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("content");

                    b.Property<long>("ConversationId")
                        .HasColumnType("bigint")
                        .HasColumnName("id_conversation");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("created_at");

                    b.Property<bool>("IsRead")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("is_read");

                    b.Property<long>("OrganizationId")
                        .HasColumnType("bigint")
                        .HasColumnName("id_organization");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("varchar(30)")
                        .HasColumnName("type");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("updated_at");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint")
                        .HasColumnName("id_user");

                    b.HasKey("Id");

                    b.HasAlternateKey("OrganizationId", "Id");

                    b.HasIndex("OrganizationId", "ConversationId");

                    b.HasIndex("OrganizationId", "UserId");

                    b.ToTable("notifications", (string)null);
                });

            modelBuilder.Entity("NovaScale.Platform.API.IdentityAccess.Domain.Model.Aggregates.Permission", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id_permission");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(80)
                        .HasColumnType("varchar(80)")
                        .HasColumnName("code");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("created_at");

                    b.Property<long>("OrganizationId")
                        .HasColumnType("bigint")
                        .HasColumnName("id_organization");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("updated_at");

                    b.HasKey("Id");

                    b.HasIndex("OrganizationId", "Code")
                        .IsUnique();

                    b.ToTable("permissions", (string)null);
                });

            modelBuilder.Entity("NovaScale.Platform.API.IdentityAccess.Domain.Model.Aggregates.Role", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id_role");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("created_at");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("description");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("name");

                    b.Property<long>("OrganizationId")
                        .HasColumnType("bigint")
                        .HasColumnName("id_organization");

                    b.Property<string>("SystemCode")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("varchar(30)")
                        .HasColumnName("system_code");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("updated_at");

                    b.HasKey("Id");

                    b.HasIndex("OrganizationId", "Name")
                        .IsUnique();

                    b.ToTable("roles", (string)null);
                });

            modelBuilder.Entity("NovaScale.Platform.API.IdentityAccess.Domain.Model.Aggregates.RolePermission", b =>
                {
                    b.Property<long>("RoleId")
                        .HasColumnType("bigint")
                        .HasColumnName("id_role");

                    b.Property<long>("PermissionId")
                        .HasColumnType("bigint")
                        .HasColumnName("id_permission");

                    b.Property<long>("OrganizationId")
                        .HasColumnType("bigint")
                        .HasColumnName("id_organization");

                    b.HasKey("RoleId", "PermissionId");

                    b.HasIndex("OrganizationId", "PermissionId");

                    b.HasIndex("OrganizationId", "RoleId");

                    b.ToTable("role_permissions", (string)null);
                });

            modelBuilder.Entity("NovaScale.Platform.API.IdentityAccess.Domain.Model.Aggregates.User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id_user");

                    b.Property<bool>("CanDeleteLeads")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("can_delete_leads");

                    b.Property<bool>("CanExportData")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("can_export_data");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("created_at");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(120)
                        .HasColumnType("varchar(120)")
                        .HasColumnName("email");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasMaxLength(120)
                        .HasColumnType("varchar(120)")
                        .HasColumnName("full_name");

                    b.Property<long>("OrganizationId")
                        .HasColumnType("bigint")
                        .HasColumnName("id_organization");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("password_hash");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("varchar(30)")
                        .HasColumnName("phone");

                    b.Property<decimal>("Quota")
                        .HasPrecision(12, 2)
                        .HasColumnType("decimal(12,2)")
                        .HasColumnName("quota");

                    b.Property<long>("RoleId")
                        .HasColumnType("bigint")
                        .HasColumnName("id_role");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("varchar(30)")
                        .HasColumnName("status");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("updated_at");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("OrganizationId", "RoleId");

                    b.ToTable("users", (string)null);
                });

            modelBuilder.Entity("NovaScale.Platform.API.LeadManagement.Domain.Model.Aggregates.Activity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id_activity");

                    b.Property<long?>("ContactId")
                        .HasColumnType("bigint")
                        .HasColumnName("id_contact");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("varchar(1000)")
                        .HasColumnName("content");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("created_at");

                    b.Property<long>("OrganizationId")
                        .HasColumnType("bigint")
                        .HasColumnName("id_organization");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("varchar(40)")
                        .HasColumnName("type");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("updated_at");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint")
                        .HasColumnName("id_user");

                    b.HasKey("Id");

                    b.HasAlternateKey("OrganizationId", "Id");

                    b.HasIndex("OrganizationId", "ContactId");

                    b.HasIndex("OrganizationId", "UserId");

                    b.ToTable("activities", (string)null);
                });

            modelBuilder.Entity("NovaScale.Platform.API.LeadManagement.Domain.Model.Aggregates.Contact", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id_contact");

                    b.Property<string>("Company")
                        .IsRequired()
                        .HasMaxLength(120)
                        .HasColumnType("varchar(120)")
                        .HasColumnName("company");

                    b.Property<string>("ContactType")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("varchar(30)")
                        .HasColumnName("contact_type");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("created_at");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(120)
                        .HasColumnType("varchar(120)")
                        .HasColumnName("email");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasMaxLength(120)
                        .HasColumnType("varchar(120)")
                        .HasColumnName("full_name");

                    b.Property<string>("LeadStatus")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("varchar(30)")
                        .HasColumnName("lead_status");

                    b.Property<long>("OrganizationId")
                        .HasColumnType("bigint")
                        .HasColumnName("id_organization");

                    b.Property<string>("Phone")
                        .HasMaxLength(30)
                        .HasColumnType("varchar(30)")
                        .HasColumnName("phone");

                    b.Property<string>("Source")
                        .IsRequired()
                        .HasMaxLength(80)
                        .HasColumnType("varchar(80)")
                        .HasColumnName("source");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("updated_at");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint")
                        .HasColumnName("id_user");

                    b.HasKey("Id");

                    b.HasIndex("OrganizationId", "Phone")
                        .IsUnique();

                    b.HasIndex("OrganizationId", "UserId");

                    b.ToTable("contacts", (string)null);
                });

            modelBuilder.Entity("NovaScale.Platform.API.LeadManagement.Domain.Model.Aggregates.ContactTag", b =>
                {
                    b.Property<long>("ContactId")
                        .HasColumnType("bigint")
                        .HasColumnName("id_contact");

                    b.Property<long>("TagId")
                        .HasColumnType("bigint")
                        .HasColumnName("id_tag");

                    b.Property<long>("OrganizationId")
                        .HasColumnType("bigint")
                        .HasColumnName("id_organization");

                    b.HasKey("ContactId", "TagId");

                    b.HasIndex("OrganizationId", "ContactId");

                    b.HasIndex("OrganizationId", "TagId");

                    b.ToTable("contact_tags", (string)null);
                });

            modelBuilder.Entity("NovaScale.Platform.API.LeadManagement.Domain.Model.Aggregates.Opportunity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id_opportunity");

                    b.Property<long>("ContactId")
                        .HasColumnType("bigint")
                        .HasColumnName("id_contact");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("created_at");

                    b.Property<decimal>("EstimatedAmount")
                        .HasPrecision(12, 2)
                        .HasColumnType("decimal(12,2)")
                        .HasColumnName("estimated_amount");

                    b.Property<DateTime?>("ExpectedCloseDate")
                        .HasColumnType("date")
                        .HasColumnName("expected_close_date");

                    b.Property<long>("OrganizationId")
                        .HasColumnType("bigint")
                        .HasColumnName("id_organization");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("varchar(30)")
                        .HasColumnName("status");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("varchar(150)")
                        .HasColumnName("title");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("updated_at");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint")
                        .HasColumnName("id_user");

                    b.HasKey("Id");

                    b.HasIndex("OrganizationId", "ContactId");

                    b.HasIndex("OrganizationId", "UserId");

                    b.ToTable("opportunities", (string)null);
                });

            modelBuilder.Entity("NovaScale.Platform.API.LeadManagement.Domain.Model.Aggregates.Tag", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id_tag");

                    b.Property<string>("Color")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("varchar(20)")
                        .HasColumnName("color");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("created_at");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("name");

                    b.Property<long>("OrganizationId")
                        .HasColumnType("bigint")
                        .HasColumnName("id_organization");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("updated_at");

                    b.HasKey("Id");

                    b.HasIndex("OrganizationId", "Name")
                        .IsUnique();

                    b.ToTable("tags", (string)null);
                });

            modelBuilder.Entity("NovaScale.Platform.API.OrganizationManagement.Domain.Model.Aggregates.Organization", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id_organization");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("created_at");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(120)
                        .HasColumnType("varchar(120)")
                        .HasColumnName("name");

                    b.HasKey("Id");

                    b.ToTable("organizations", (string)null);
                });

            modelBuilder.Entity("NovaScale.Platform.API.SalesManagement.Domain.Model.Aggregates.Sale", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id_sale");

                    b.Property<decimal>("Amount")
                        .HasPrecision(12, 2)
                        .HasColumnType("decimal(12,2)")
                        .HasColumnName("amount");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("created_at");

                    b.Property<string>("Notes")
                        .IsRequired()
                        .HasMaxLength(4000)
                        .HasColumnType("text")
                        .HasColumnName("notes");

                    b.Property<long>("OpportunityId")
                        .HasColumnType("bigint")
                        .HasColumnName("id_opportunity");

                    b.Property<long>("OrganizationId")
                        .HasColumnType("bigint")
                        .HasColumnName("id_organization");

                    b.Property<DateTime>("SaleDate")
                        .HasColumnType("date")
                        .HasColumnName("sale_date");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("varchar(30)")
                        .HasColumnName("status");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("updated_at");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint")
                        .HasColumnName("id_user");

                    b.HasKey("Id");

                    b.HasAlternateKey("OrganizationId", "Id");

                    b.HasIndex("OpportunityId")
                        .IsUnique();

                    b.HasIndex("OrganizationId", "OpportunityId");

                    b.HasIndex("OrganizationId", "UserId");

                    b.ToTable("sales", (string)null);
                });

            modelBuilder.Entity("NovaScale.Platform.API.ConversationManagement.Domain.Model.Aggregates.Conversation", b =>
                {
                    b.HasOne("NovaScale.Platform.API.OrganizationManagement.Domain.Model.Aggregates.Organization", null)
                        .WithMany()
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("NovaScale.Platform.API.LeadManagement.Domain.Model.Aggregates.Contact", null)
                        .WithMany()
                        .HasForeignKey("OrganizationId", "ContactId")
                        .HasPrincipalKey("OrganizationId", "Id")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("NovaScale.Platform.API.IdentityAccess.Domain.Model.Aggregates.User", null)
                        .WithMany()
                        .HasForeignKey("OrganizationId", "UserId")
                        .HasPrincipalKey("OrganizationId", "Id")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("NovaScale.Platform.API.ConversationManagement.Domain.Model.Aggregates.Message", b =>
                {
                    b.HasOne("NovaScale.Platform.API.OrganizationManagement.Domain.Model.Aggregates.Organization", null)
                        .WithMany()
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("NovaScale.Platform.API.ConversationManagement.Domain.Model.Aggregates.Conversation", null)
                        .WithMany()
                        .HasForeignKey("OrganizationId", "ConversationId")
                        .HasPrincipalKey("OrganizationId", "Id")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("NovaScale.Platform.API.IdentityAccess.Domain.Model.Aggregates.User", null)
                        .WithMany()
                        .HasForeignKey("OrganizationId", "UserId")
                        .HasPrincipalKey("OrganizationId", "Id")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("NovaScale.Platform.API.ConversationManagement.Domain.Model.Aggregates.Notification", b =>
                {
                    b.HasOne("NovaScale.Platform.API.OrganizationManagement.Domain.Model.Aggregates.Organization", null)
                        .WithMany()
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("NovaScale.Platform.API.ConversationManagement.Domain.Model.Aggregates.Conversation", null)
                        .WithMany()
                        .HasForeignKey("OrganizationId", "ConversationId")
                        .HasPrincipalKey("OrganizationId", "Id")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("NovaScale.Platform.API.IdentityAccess.Domain.Model.Aggregates.User", null)
                        .WithMany()
                        .HasForeignKey("OrganizationId", "UserId")
                        .HasPrincipalKey("OrganizationId", "Id")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("NovaScale.Platform.API.IdentityAccess.Domain.Model.Aggregates.Permission", b =>
                {
                    b.HasOne("NovaScale.Platform.API.OrganizationManagement.Domain.Model.Aggregates.Organization", null)
                        .WithMany()
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("NovaScale.Platform.API.IdentityAccess.Domain.Model.Aggregates.Role", b =>
                {
                    b.HasOne("NovaScale.Platform.API.OrganizationManagement.Domain.Model.Aggregates.Organization", null)
                        .WithMany()
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("NovaScale.Platform.API.IdentityAccess.Domain.Model.Aggregates.RolePermission", b =>
                {
                    b.HasOne("NovaScale.Platform.API.IdentityAccess.Domain.Model.Aggregates.Permission", null)
                        .WithMany()
                        .HasForeignKey("OrganizationId", "PermissionId")
                        .HasPrincipalKey("OrganizationId", "Id")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("NovaScale.Platform.API.IdentityAccess.Domain.Model.Aggregates.Role", null)
                        .WithMany()
                        .HasForeignKey("OrganizationId", "RoleId")
                        .HasPrincipalKey("OrganizationId", "Id")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("NovaScale.Platform.API.IdentityAccess.Domain.Model.Aggregates.User", b =>
                {
                    b.HasOne("NovaScale.Platform.API.OrganizationManagement.Domain.Model.Aggregates.Organization", null)
                        .WithMany()
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("NovaScale.Platform.API.IdentityAccess.Domain.Model.Aggregates.Role", null)
                        .WithMany()
                        .HasForeignKey("OrganizationId", "RoleId")
                        .HasPrincipalKey("OrganizationId", "Id")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("NovaScale.Platform.API.LeadManagement.Domain.Model.Aggregates.Activity", b =>
                {
                    b.HasOne("NovaScale.Platform.API.OrganizationManagement.Domain.Model.Aggregates.Organization", null)
                        .WithMany()
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("NovaScale.Platform.API.LeadManagement.Domain.Model.Aggregates.Contact", null)
                        .WithMany()
                        .HasForeignKey("OrganizationId", "ContactId")
                        .HasPrincipalKey("OrganizationId", "Id")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("NovaScale.Platform.API.IdentityAccess.Domain.Model.Aggregates.User", null)
                        .WithMany()
                        .HasForeignKey("OrganizationId", "UserId")
                        .HasPrincipalKey("OrganizationId", "Id")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("NovaScale.Platform.API.LeadManagement.Domain.Model.Aggregates.Contact", b =>
                {
                    b.HasOne("NovaScale.Platform.API.OrganizationManagement.Domain.Model.Aggregates.Organization", null)
                        .WithMany()
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("NovaScale.Platform.API.IdentityAccess.Domain.Model.Aggregates.User", null)
                        .WithMany()
                        .HasForeignKey("OrganizationId", "UserId")
                        .HasPrincipalKey("OrganizationId", "Id")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("NovaScale.Platform.API.LeadManagement.Domain.Model.Aggregates.ContactTag", b =>
                {
                    b.HasOne("NovaScale.Platform.API.LeadManagement.Domain.Model.Aggregates.Contact", null)
                        .WithMany()
                        .HasForeignKey("OrganizationId", "ContactId")
                        .HasPrincipalKey("OrganizationId", "Id")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("NovaScale.Platform.API.LeadManagement.Domain.Model.Aggregates.Tag", null)
                        .WithMany()
                        .HasForeignKey("OrganizationId", "TagId")
                        .HasPrincipalKey("OrganizationId", "Id")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("NovaScale.Platform.API.LeadManagement.Domain.Model.Aggregates.Opportunity", b =>
                {
                    b.HasOne("NovaScale.Platform.API.OrganizationManagement.Domain.Model.Aggregates.Organization", null)
                        .WithMany()
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("NovaScale.Platform.API.LeadManagement.Domain.Model.Aggregates.Contact", null)
                        .WithMany()
                        .HasForeignKey("OrganizationId", "ContactId")
                        .HasPrincipalKey("OrganizationId", "Id")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("NovaScale.Platform.API.IdentityAccess.Domain.Model.Aggregates.User", null)
                        .WithMany()
                        .HasForeignKey("OrganizationId", "UserId")
                        .HasPrincipalKey("OrganizationId", "Id")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("NovaScale.Platform.API.LeadManagement.Domain.Model.Aggregates.Tag", b =>
                {
                    b.HasOne("NovaScale.Platform.API.OrganizationManagement.Domain.Model.Aggregates.Organization", null)
                        .WithMany()
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("NovaScale.Platform.API.SalesManagement.Domain.Model.Aggregates.Sale", b =>
                {
                    b.HasOne("NovaScale.Platform.API.OrganizationManagement.Domain.Model.Aggregates.Organization", null)
                        .WithMany()
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("NovaScale.Platform.API.LeadManagement.Domain.Model.Aggregates.Opportunity", null)
                        .WithMany()
                        .HasForeignKey("OrganizationId", "OpportunityId")
                        .HasPrincipalKey("OrganizationId", "Id")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("NovaScale.Platform.API.IdentityAccess.Domain.Model.Aggregates.User", null)
                        .WithMany()
                        .HasForeignKey("OrganizationId", "UserId")
                        .HasPrincipalKey("OrganizationId", "Id")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
