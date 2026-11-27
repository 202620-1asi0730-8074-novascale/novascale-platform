using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace NovaScale.Platform.API.Shared.Infrastructure.Persistence.EFC.Migrations
{
    /// <inheritdoc />
    public partial class AddGmailIntegration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "client_request_id",
                table: "messages",
                type: "char(36)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "delivery_status",
                table: "messages",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "LOCAL");

            migrationBuilder.AddColumn<string>(
                name: "email_message_id",
                table: "messages",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "gmail_mailbox",
                table: "conversations",
                type: "varchar(120)",
                maxLength: 120,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "gmail_recipient_email",
                table: "conversations",
                type: "varchar(120)",
                maxLength: 120,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "gmail_subject",
                table: "conversations",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "gmail_thread_id",
                table: "conversations",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "gmail_accounts",
                columns: table => new
                {
                    id_gmail_account = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    connected_by_user_id = table.Column<long>(type: "bigint", nullable: false),
                    email = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false),
                    protected_refresh_token = table.Column<string>(type: "text", nullable: false),
                    protected_access_token = table.Column<string>(type: "text", nullable: false),
                    access_token_expires_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    id_organization = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gmail_accounts", x => x.id_gmail_account);
                    table.ForeignKey(
                        name: "FK_gmail_accounts_organizations_id_organization",
                        column: x => x.id_organization,
                        principalTable: "organizations",
                        principalColumn: "id_organization",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_gmail_accounts_users_id_organization_connected_by_user_id",
                        columns: x => new { x.id_organization, x.connected_by_user_id },
                        principalTable: "users",
                        principalColumns: new[] { "id_organization", "id_user" },
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_messages_id_organization_client_request_id",
                table: "messages",
                columns: new[] { "id_organization", "client_request_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_gmail_accounts_id_organization",
                table: "gmail_accounts",
                column: "id_organization",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_gmail_accounts_id_organization_connected_by_user_id",
                table: "gmail_accounts",
                columns: new[] { "id_organization", "connected_by_user_id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "gmail_accounts");

            migrationBuilder.DropIndex(
                name: "IX_messages_id_organization_client_request_id",
                table: "messages");

            migrationBuilder.DropColumn(
                name: "client_request_id",
                table: "messages");

            migrationBuilder.DropColumn(
                name: "delivery_status",
                table: "messages");

            migrationBuilder.DropColumn(
                name: "email_message_id",
                table: "messages");

            migrationBuilder.DropColumn(
                name: "gmail_mailbox",
                table: "conversations");

            migrationBuilder.DropColumn(
                name: "gmail_recipient_email",
                table: "conversations");

            migrationBuilder.DropColumn(
                name: "gmail_subject",
                table: "conversations");

            migrationBuilder.DropColumn(
                name: "gmail_thread_id",
                table: "conversations");
        }
    }
}
