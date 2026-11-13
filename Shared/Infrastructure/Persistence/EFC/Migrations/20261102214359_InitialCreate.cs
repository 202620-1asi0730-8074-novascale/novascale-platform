using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace NovaScale.Platform.API.Shared.Infrastructure.Persistence.EFC.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "organizations",
                columns: table => new
                {
                    id_organization = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_organizations", x => x.id_organization);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "permissions",
                columns: table => new
                {
                    id_permission = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    code = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false),
                    id_organization = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_permissions", x => x.id_permission);
                    table.UniqueConstraint("AK_permissions_id_organization_id_permission", x => new { x.id_organization, x.id_permission });
                    table.ForeignKey(
                        name: "FK_permissions_organizations_id_organization",
                        column: x => x.id_organization,
                        principalTable: "organizations",
                        principalColumn: "id_organization",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id_role = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    system_code = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    id_organization = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.id_role);
                    table.UniqueConstraint("AK_roles_id_organization_id_role", x => new { x.id_organization, x.id_role });
                    table.ForeignKey(
                        name: "FK_roles_organizations_id_organization",
                        column: x => x.id_organization,
                        principalTable: "organizations",
                        principalColumn: "id_organization",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tags",
                columns: table => new
                {
                    id_tag = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    color = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    id_organization = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tags", x => x.id_tag);
                    table.UniqueConstraint("AK_tags_id_organization_id_tag", x => new { x.id_organization, x.id_tag });
                    table.ForeignKey(
                        name: "FK_tags_organizations_id_organization",
                        column: x => x.id_organization,
                        principalTable: "organizations",
                        principalColumn: "id_organization",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "role_permissions",
                columns: table => new
                {
                    id_role = table.Column<long>(type: "bigint", nullable: false),
                    id_permission = table.Column<long>(type: "bigint", nullable: false),
                    id_organization = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role_permissions", x => new { x.id_role, x.id_permission });
                    table.ForeignKey(
                        name: "FK_role_permissions_permissions_id_organization_id_permission",
                        columns: x => new { x.id_organization, x.id_permission },
                        principalTable: "permissions",
                        principalColumns: new[] { "id_organization", "id_permission" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_role_permissions_roles_id_organization_id_role",
                        columns: x => new { x.id_organization, x.id_role },
                        principalTable: "roles",
                        principalColumns: new[] { "id_organization", "id_role" },
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id_user = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    full_name = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false),
                    email = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false),
                    phone = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    quota = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    status = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    id_role = table.Column<long>(type: "bigint", nullable: false),
                    can_export_data = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    can_delete_leads = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    password_hash = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    id_organization = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id_user);
                    table.UniqueConstraint("AK_users_id_organization_id_user", x => new { x.id_organization, x.id_user });
                    table.ForeignKey(
                        name: "FK_users_organizations_id_organization",
                        column: x => x.id_organization,
                        principalTable: "organizations",
                        principalColumn: "id_organization",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_users_roles_id_organization_id_role",
                        columns: x => new { x.id_organization, x.id_role },
                        principalTable: "roles",
                        principalColumns: new[] { "id_organization", "id_role" },
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "contacts",
                columns: table => new
                {
                    id_contact = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    full_name = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false),
                    email = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false),
                    phone = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true),
                    company = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false),
                    contact_type = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    lead_status = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    source = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false),
                    id_user = table.Column<long>(type: "bigint", nullable: false),
                    id_organization = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contacts", x => x.id_contact);
                    table.UniqueConstraint("AK_contacts_id_organization_id_contact", x => new { x.id_organization, x.id_contact });
                    table.ForeignKey(
                        name: "FK_contacts_organizations_id_organization",
                        column: x => x.id_organization,
                        principalTable: "organizations",
                        principalColumn: "id_organization",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_contacts_users_id_organization_id_user",
                        columns: x => new { x.id_organization, x.id_user },
                        principalTable: "users",
                        principalColumns: new[] { "id_organization", "id_user" },
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "activities",
                columns: table => new
                {
                    id_activity = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    type = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    content = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false),
                    id_user = table.Column<long>(type: "bigint", nullable: false),
                    id_contact = table.Column<long>(type: "bigint", nullable: true),
                    id_organization = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_activities", x => x.id_activity);
                    table.UniqueConstraint("AK_activities_id_organization_id_activity", x => new { x.id_organization, x.id_activity });
                    table.ForeignKey(
                        name: "FK_activities_contacts_id_organization_id_contact",
                        columns: x => new { x.id_organization, x.id_contact },
                        principalTable: "contacts",
                        principalColumns: new[] { "id_organization", "id_contact" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_activities_organizations_id_organization",
                        column: x => x.id_organization,
                        principalTable: "organizations",
                        principalColumn: "id_organization",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_activities_users_id_organization_id_user",
                        columns: x => new { x.id_organization, x.id_user },
                        principalTable: "users",
                        principalColumns: new[] { "id_organization", "id_user" },
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "contact_tags",
                columns: table => new
                {
                    id_contact = table.Column<long>(type: "bigint", nullable: false),
                    id_tag = table.Column<long>(type: "bigint", nullable: false),
                    id_organization = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contact_tags", x => new { x.id_contact, x.id_tag });
                    table.ForeignKey(
                        name: "FK_contact_tags_contacts_id_organization_id_contact",
                        columns: x => new { x.id_organization, x.id_contact },
                        principalTable: "contacts",
                        principalColumns: new[] { "id_organization", "id_contact" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_contact_tags_tags_id_organization_id_tag",
                        columns: x => new { x.id_organization, x.id_tag },
                        principalTable: "tags",
                        principalColumns: new[] { "id_organization", "id_tag" },
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "conversations",
                columns: table => new
                {
                    id_conversation = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    channel = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    status = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    id_contact = table.Column<long>(type: "bigint", nullable: false),
                    id_user = table.Column<long>(type: "bigint", nullable: false),
                    priority = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false),
                    pending_since = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    last_message_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    id_organization = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_conversations", x => x.id_conversation);
                    table.UniqueConstraint("AK_conversations_id_organization_id_conversation", x => new { x.id_organization, x.id_conversation });
                    table.ForeignKey(
                        name: "FK_conversations_contacts_id_organization_id_contact",
                        columns: x => new { x.id_organization, x.id_contact },
                        principalTable: "contacts",
                        principalColumns: new[] { "id_organization", "id_contact" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_conversations_organizations_id_organization",
                        column: x => x.id_organization,
                        principalTable: "organizations",
                        principalColumn: "id_organization",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_conversations_users_id_organization_id_user",
                        columns: x => new { x.id_organization, x.id_user },
                        principalTable: "users",
                        principalColumns: new[] { "id_organization", "id_user" },
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "opportunities",
                columns: table => new
                {
                    id_opportunity = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    title = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false),
                    estimated_amount = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    status = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    expected_close_date = table.Column<DateOnly>(type: "date", nullable: true),
                    id_user = table.Column<long>(type: "bigint", nullable: false),
                    id_contact = table.Column<long>(type: "bigint", nullable: false),
                    id_organization = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_opportunities", x => x.id_opportunity);
                    table.UniqueConstraint("AK_opportunities_id_organization_id_opportunity", x => new { x.id_organization, x.id_opportunity });
                    table.ForeignKey(
                        name: "FK_opportunities_contacts_id_organization_id_contact",
                        columns: x => new { x.id_organization, x.id_contact },
                        principalTable: "contacts",
                        principalColumns: new[] { "id_organization", "id_contact" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_opportunities_organizations_id_organization",
                        column: x => x.id_organization,
                        principalTable: "organizations",
                        principalColumn: "id_organization",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_opportunities_users_id_organization_id_user",
                        columns: x => new { x.id_organization, x.id_user },
                        principalTable: "users",
                        principalColumns: new[] { "id_organization", "id_user" },
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "messages",
                columns: table => new
                {
                    id_message = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    sender_type = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    content = table.Column<string>(type: "text", maxLength: 16000, nullable: false),
                    id_conversation = table.Column<long>(type: "bigint", nullable: false),
                    id_user = table.Column<long>(type: "bigint", nullable: false),
                    sent_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    external_message_id = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: true),
                    attachments_json = table.Column<string>(type: "longtext", nullable: false),
                    id_organization = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_messages", x => x.id_message);
                    table.UniqueConstraint("AK_messages_id_organization_id_message", x => new { x.id_organization, x.id_message });
                    table.ForeignKey(
                        name: "FK_messages_conversations_id_organization_id_conversation",
                        columns: x => new { x.id_organization, x.id_conversation },
                        principalTable: "conversations",
                        principalColumns: new[] { "id_organization", "id_conversation" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_messages_organizations_id_organization",
                        column: x => x.id_organization,
                        principalTable: "organizations",
                        principalColumn: "id_organization",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_messages_users_id_organization_id_user",
                        columns: x => new { x.id_organization, x.id_user },
                        principalTable: "users",
                        principalColumns: new[] { "id_organization", "id_user" },
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "notifications",
                columns: table => new
                {
                    id_notification = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    type = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    content = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    is_read = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    id_user = table.Column<long>(type: "bigint", nullable: false),
                    id_conversation = table.Column<long>(type: "bigint", nullable: false),
                    id_organization = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notifications", x => x.id_notification);
                    table.UniqueConstraint("AK_notifications_id_organization_id_notification", x => new { x.id_organization, x.id_notification });
                    table.ForeignKey(
                        name: "FK_notifications_conversations_id_organization_id_conversation",
                        columns: x => new { x.id_organization, x.id_conversation },
                        principalTable: "conversations",
                        principalColumns: new[] { "id_organization", "id_conversation" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_notifications_organizations_id_organization",
                        column: x => x.id_organization,
                        principalTable: "organizations",
                        principalColumn: "id_organization",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_notifications_users_id_organization_id_user",
                        columns: x => new { x.id_organization, x.id_user },
                        principalTable: "users",
                        principalColumns: new[] { "id_organization", "id_user" },
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "sales",
                columns: table => new
                {
                    id_sale = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    amount = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    sale_date = table.Column<DateOnly>(type: "date", nullable: false),
                    status = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    notes = table.Column<string>(type: "text", maxLength: 4000, nullable: false),
                    id_user = table.Column<long>(type: "bigint", nullable: false),
                    id_opportunity = table.Column<long>(type: "bigint", nullable: false),
                    id_organization = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sales", x => x.id_sale);
                    table.UniqueConstraint("AK_sales_id_organization_id_sale", x => new { x.id_organization, x.id_sale });
                    table.ForeignKey(
                        name: "FK_sales_opportunities_id_organization_id_opportunity",
                        columns: x => new { x.id_organization, x.id_opportunity },
                        principalTable: "opportunities",
                        principalColumns: new[] { "id_organization", "id_opportunity" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_sales_organizations_id_organization",
                        column: x => x.id_organization,
                        principalTable: "organizations",
                        principalColumn: "id_organization",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_sales_users_id_organization_id_user",
                        columns: x => new { x.id_organization, x.id_user },
                        principalTable: "users",
                        principalColumns: new[] { "id_organization", "id_user" },
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_activities_id_organization_id_contact",
                table: "activities",
                columns: new[] { "id_organization", "id_contact" });

            migrationBuilder.CreateIndex(
                name: "IX_activities_id_organization_id_user",
                table: "activities",
                columns: new[] { "id_organization", "id_user" });

            migrationBuilder.CreateIndex(
                name: "IX_contact_tags_id_organization_id_contact",
                table: "contact_tags",
                columns: new[] { "id_organization", "id_contact" });

            migrationBuilder.CreateIndex(
                name: "IX_contact_tags_id_organization_id_tag",
                table: "contact_tags",
                columns: new[] { "id_organization", "id_tag" });

            migrationBuilder.CreateIndex(
                name: "IX_contacts_id_organization_id_user",
                table: "contacts",
                columns: new[] { "id_organization", "id_user" });

            migrationBuilder.CreateIndex(
                name: "IX_contacts_id_organization_phone",
                table: "contacts",
                columns: new[] { "id_organization", "phone" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_conversations_id_organization_id_contact",
                table: "conversations",
                columns: new[] { "id_organization", "id_contact" });

            migrationBuilder.CreateIndex(
                name: "IX_conversations_id_organization_id_user",
                table: "conversations",
                columns: new[] { "id_organization", "id_user" });

            migrationBuilder.CreateIndex(
                name: "IX_messages_id_organization_external_message_id",
                table: "messages",
                columns: new[] { "id_organization", "external_message_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_messages_id_organization_id_conversation",
                table: "messages",
                columns: new[] { "id_organization", "id_conversation" });

            migrationBuilder.CreateIndex(
                name: "IX_messages_id_organization_id_user",
                table: "messages",
                columns: new[] { "id_organization", "id_user" });

            migrationBuilder.CreateIndex(
                name: "IX_notifications_id_organization_id_conversation",
                table: "notifications",
                columns: new[] { "id_organization", "id_conversation" });

            migrationBuilder.CreateIndex(
                name: "IX_notifications_id_organization_id_user",
                table: "notifications",
                columns: new[] { "id_organization", "id_user" });

            migrationBuilder.CreateIndex(
                name: "IX_opportunities_id_organization_id_contact",
                table: "opportunities",
                columns: new[] { "id_organization", "id_contact" });

            migrationBuilder.CreateIndex(
                name: "IX_opportunities_id_organization_id_user",
                table: "opportunities",
                columns: new[] { "id_organization", "id_user" });

            migrationBuilder.CreateIndex(
                name: "IX_permissions_id_organization_code",
                table: "permissions",
                columns: new[] { "id_organization", "code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_role_permissions_id_organization_id_permission",
                table: "role_permissions",
                columns: new[] { "id_organization", "id_permission" });

            migrationBuilder.CreateIndex(
                name: "IX_role_permissions_id_organization_id_role",
                table: "role_permissions",
                columns: new[] { "id_organization", "id_role" });

            migrationBuilder.CreateIndex(
                name: "IX_roles_id_organization_name",
                table: "roles",
                columns: new[] { "id_organization", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sales_id_opportunity",
                table: "sales",
                column: "id_opportunity",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sales_id_organization_id_opportunity",
                table: "sales",
                columns: new[] { "id_organization", "id_opportunity" });

            migrationBuilder.CreateIndex(
                name: "IX_sales_id_organization_id_user",
                table: "sales",
                columns: new[] { "id_organization", "id_user" });

            migrationBuilder.CreateIndex(
                name: "IX_tags_id_organization_name",
                table: "tags",
                columns: new[] { "id_organization", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_email",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_id_organization_id_role",
                table: "users",
                columns: new[] { "id_organization", "id_role" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "activities");

            migrationBuilder.DropTable(
                name: "contact_tags");

            migrationBuilder.DropTable(
                name: "messages");

            migrationBuilder.DropTable(
                name: "notifications");

            migrationBuilder.DropTable(
                name: "role_permissions");

            migrationBuilder.DropTable(
                name: "sales");

            migrationBuilder.DropTable(
                name: "tags");

            migrationBuilder.DropTable(
                name: "conversations");

            migrationBuilder.DropTable(
                name: "permissions");

            migrationBuilder.DropTable(
                name: "opportunities");

            migrationBuilder.DropTable(
                name: "contacts");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "organizations");
        }
    }
}
