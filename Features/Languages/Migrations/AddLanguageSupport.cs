using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Features.Languages.Migrations;

public partial class AddLanguageSupport : Migration
{
  protected override void Up(MigrationBuilder migrationBuilder)
  {
    // Create Languages table
    migrationBuilder.CreateTable(
        name: "languages",
        columns: table => new
        {
          id = table.Column<Guid>(type: "uuid", nullable: false),
          iso_code = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
          name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
          is_active = table.Column<bool>(type: "boolean", nullable: false),
          is_default = table.Column<bool>(type: "boolean", nullable: false),
          flag_icon = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
          created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
          created_by_id = table.Column<string>(type: "text", nullable: true),
          modified_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
          modified_by_id = table.Column<string>(type: "text", nullable: true),
          deleted_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
          deleted_by_id = table.Column<string>(type: "text", nullable: true),
          is_deleted = table.Column<bool>(type: "boolean", nullable: false)
        },
        constraints: table =>
        {
          table.PrimaryKey("pk_languages", x => x.id);
        });

    // Create MediaTranslations table
    migrationBuilder.CreateTable(
        name: "media_translations",
        columns: table => new
        {
          id = table.Column<Guid>(type: "uuid", nullable: false),
          media_id = table.Column<Guid>(type: "uuid", nullable: false),
          language_code = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
          title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
          overview = table.Column<string>(type: "text", nullable: true),
          slug = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
          meta_data = table.Column<JsonDocument>(type: "jsonb", nullable: true),
          created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
          created_by_id = table.Column<string>(type: "text", nullable: true),
          modified_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
          modified_by_id = table.Column<string>(type: "text", nullable: true),
          deleted_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
          deleted_by_id = table.Column<string>(type: "text", nullable: true),
          is_deleted = table.Column<bool>(type: "boolean", nullable: false)
        },
        constraints: table =>
        {
          table.PrimaryKey("pk_media_translations", x => x.id);
          table.ForeignKey(
                  name: "fk_media_translations_languages_language_code",
                  column: x => x.language_code,
                  principalTable: "languages",
                  principalColumn: "iso_code",
                  onDelete: ReferentialAction.Cascade);
        });

    // Add indexes
    migrationBuilder.CreateIndex(
        name: "ix_languages_iso_code",
        table: "languages",
        column: "iso_code",
        unique: true);

    migrationBuilder.CreateIndex(
        name: "ix_media_translations_language_code",
        table: "media_translations",
        column: "language_code");

    migrationBuilder.CreateIndex(
        name: "ix_media_translations_media_id",
        table: "media_translations",
        column: "media_id");

    migrationBuilder.CreateIndex(
        name: "ix_media_translations_slug",
        table: "media_translations",
        column: "slug");

    // Insert default language
    migrationBuilder.InsertData(
        table: "languages",
        columns: new[] { "id", "iso_code", "name", "is_active", "is_default", "created_on", "is_deleted" },
        values: new object[] { Guid.NewGuid(), "de", "Deutsch", true, true, DateTime.UtcNow, false });
  }

  protected override void Down(MigrationBuilder migrationBuilder)
  {
    migrationBuilder.DropTable(
        name: "media_translations");

    migrationBuilder.DropTable(
        name: "languages");
  }
}