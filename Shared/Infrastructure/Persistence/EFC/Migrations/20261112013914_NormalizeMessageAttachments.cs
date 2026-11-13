using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace NovaScale.Platform.API.Shared.Infrastructure.Persistence.EFC.Migrations
{
    public partial class NormalizeMessageAttachments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "message_attachments",
                columns: table => new
                {
                    id_attachment = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    content_type = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    data = table.Column<string>(type: "longtext", nullable: false),
                    size_bytes = table.Column<int>(type: "int", nullable: false),
                    id_message = table.Column<long>(type: "bigint", nullable: false),
                    id_organization = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_message_attachments", x => x.id_attachment);
                    table.UniqueConstraint("AK_message_attachments_id_organization_id_attachment", x => new { x.id_organization, x.id_attachment });
                    table.ForeignKey(
                        name: "FK_message_attachments_messages_id_organization_id_message",
                        columns: x => new { x.id_organization, x.id_message },
                        principalTable: "messages",
                        principalColumns: new[] { "id_organization", "id_message" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_message_attachments_organizations_id_organization",
                        column: x => x.id_organization,
                        principalTable: "organizations",
                        principalColumn: "id_organization",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_message_attachments_id_organization_id_message",
                table: "message_attachments",
                columns: new[] { "id_organization", "id_message" });

            migrationBuilder.Sql("""
                INSERT INTO message_attachments (id_message, id_organization, name, content_type, data, size_bytes, created_at, updated_at)
                SELECT m.id_message, m.id_organization, COALESCE(j.upper_name, j.lower_name), COALESCE(j.upper_type, j.lower_type),
                    COALESCE(j.upper_data, j.lower_data), OCTET_LENGTH(FROM_BASE64(SUBSTRING_INDEX(COALESCE(j.upper_data, j.lower_data), ',', -1))),
                    m.created_at, m.updated_at
                FROM messages AS m
                CROSS JOIN JSON_TABLE(COALESCE(NULLIF(m.attachments_json, ''), '[]'), '$[*]' COLUMNS (
                    upper_name VARCHAR(255) PATH '$.Name' NULL ON EMPTY,
                    lower_name VARCHAR(255) PATH '$.name' NULL ON EMPTY,
                    upper_type VARCHAR(100) PATH '$.ContentType' NULL ON EMPTY,
                    lower_type VARCHAR(100) PATH '$.contentType' NULL ON EMPTY,
                    upper_data LONGTEXT PATH '$.Data' NULL ON EMPTY,
                    lower_data LONGTEXT PATH '$.data' NULL ON EMPTY
                )) AS j;
                """);

            migrationBuilder.DropColumn(
                name: "attachments_json",
                table: "messages");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "attachments_json",
                table: "messages",
                type: "longtext",
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE messages AS m SET attachments_json = COALESCE((
                    SELECT JSON_ARRAYAGG(JSON_OBJECT('Name', a.name, 'ContentType', a.content_type, 'Data', a.data))
                    FROM message_attachments AS a
                    WHERE a.id_message = m.id_message AND a.id_organization = m.id_organization
                ), JSON_ARRAY());
                """);

            migrationBuilder.Sql("ALTER TABLE messages MODIFY attachments_json LONGTEXT NOT NULL;");

            migrationBuilder.DropTable(
                name: "message_attachments");
        }
    }
}
