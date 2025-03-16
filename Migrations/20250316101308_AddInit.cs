using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class AddInit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:hstore", ",,");

            migrationBuilder.CreateTable(
                name: "asp_net_users",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    first_name = table.Column<string>(type: "text", nullable: true),
                    last_name = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_login_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    user_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalized_user_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalized_email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    email_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: true),
                    security_stamp = table.Column<string>(type: "text", nullable: true),
                    concurrency_stamp = table.Column<string>(type: "text", nullable: true),
                    phone_number = table.Column<string>(type: "text", nullable: true),
                    phone_number_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    two_factor_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    lockout_end = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    lockout_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    access_failed_count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_asp_net_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ab_tests",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    feature_key = table.Column<string>(type: "text", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    end_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    VariantDistribution = table.Column<Dictionary<string, double>>(type: "jsonb", nullable: false),
                    VariantConfigurations = table.Column<Dictionary<string, JsonDocument>>(type: "jsonb", nullable: false),
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
                    table.PrimaryKey("PK_ab_tests", x => x.id);
                    table.ForeignKey(
                        name: "FK_ab_tests_asp_net_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ab_tests_asp_net_users_deleted_by_id",
                        column: x => x.deleted_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ab_tests_asp_net_users_modified_by_id",
                        column: x => x.modified_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_asp_net_users_UserId",
                        column: x => x.UserId,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_asp_net_users_UserId",
                        column: x => x.UserId,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_asp_net_users_UserId",
                        column: x => x.UserId,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "collections",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    is_public = table.Column<bool>(type: "boolean", nullable: false),
                    owner_id = table.Column<string>(type: "text", nullable: false),
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
                    table.PrimaryKey("PK_collections", x => x.id);
                    table.ForeignKey(
                        name: "FK_collections_asp_net_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_collections_asp_net_users_deleted_by_id",
                        column: x => x.deleted_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_collections_asp_net_users_modified_by_id",
                        column: x => x.modified_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_collections_asp_net_users_owner_id",
                        column: x => x.owner_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "feature_flags",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    key = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    category = table.Column<string>(type: "text", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    Configuration = table.Column<Dictionary<string, object>>(type: "jsonb", nullable: true),
                    client_key = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("PK_feature_flags", x => x.id);
                    table.ForeignKey(
                        name: "FK_feature_flags_asp_net_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_feature_flags_asp_net_users_deleted_by_id",
                        column: x => x.deleted_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_feature_flags_asp_net_users_modified_by_id",
                        column: x => x.modified_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "genres",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tmdb_id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
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
                    table.PrimaryKey("PK_genres", x => x.id);
                    table.ForeignKey(
                        name: "FK_genres_asp_net_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_genres_asp_net_users_deleted_by_id",
                        column: x => x.deleted_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_genres_asp_net_users_modified_by_id",
                        column: x => x.modified_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "jobs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    schedule_type = table.Column<int>(type: "integer", nullable: false),
                    cron_expression = table.Column<string>(type: "text", nullable: true),
                    last_run = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    next_run = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    error_message = table.Column<string>(type: "text", nullable: true),
                    parameters = table.Column<Dictionary<string, string>>(type: "hstore", nullable: true),
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
                    table.PrimaryKey("PK_jobs", x => x.id);
                    table.ForeignKey(
                        name: "FK_jobs_asp_net_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_jobs_asp_net_users_deleted_by_id",
                        column: x => x.deleted_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_jobs_asp_net_users_modified_by_id",
                        column: x => x.modified_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "languages",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    iso_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
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
                    table.PrimaryKey("PK_languages", x => x.id);
                    table.ForeignKey(
                        name: "FK_languages_asp_net_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_languages_asp_net_users_deleted_by_id",
                        column: x => x.deleted_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_languages_asp_net_users_modified_by_id",
                        column: x => x.modified_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Movies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    OriginalTitle = table.Column<string>(type: "text", nullable: true),
                    ReleaseDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Runtime = table.Column<int>(type: "integer", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedById = table.Column<string>(type: "text", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifiedById = table.Column<string>(type: "text", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedById = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    TmdbId = table.Column<int>(type: "integer", nullable: false),
                    Slug = table.Column<string>(type: "text", nullable: true),
                    Overview = table.Column<string>(type: "text", nullable: true),
                    PosterPath = table.Column<string>(type: "text", nullable: true),
                    BackdropPath = table.Column<string>(type: "text", nullable: true),
                    VoteAverage = table.Column<decimal>(type: "numeric", nullable: true),
                    VoteCount = table.Column<int>(type: "integer", nullable: true),
                    Popularity = table.Column<decimal>(type: "numeric", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Movies_asp_net_users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "asp_net_users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_Movies_asp_net_users_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "asp_net_users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_Movies_asp_net_users_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "asp_net_users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "page_views",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    path = table.Column<string>(type: "text", nullable: false),
                    referrer_url = table.Column<string>(type: "text", nullable: true),
                    user_agent = table.Column<string>(type: "text", nullable: true),
                    ip_address = table.Column<string>(type: "text", nullable: true),
                    session_id = table.Column<string>(type: "text", nullable: true),
                    user_id = table.Column<string>(type: "text", nullable: true),
                    duration = table.Column<TimeSpan>(type: "interval", nullable: true),
                    custom_data = table.Column<Dictionary<string, string>>(type: "hstore", nullable: true),
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
                    table.PrimaryKey("PK_page_views", x => x.id);
                    table.ForeignKey(
                        name: "FK_page_views_asp_net_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_page_views_asp_net_users_deleted_by_id",
                        column: x => x.deleted_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_page_views_asp_net_users_modified_by_id",
                        column: x => x.modified_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "pages",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    seo_title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    meta_description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    slug = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    link_target = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    components = table.Column<string>(type: "jsonb", nullable: false),
                    version = table.Column<int>(type: "integer", nullable: false),
                    is_published = table.Column<bool>(type: "boolean", nullable: false),
                    original_page_id = table.Column<Guid>(type: "uuid", nullable: true),
                    published_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    published_by_id = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("PK_pages", x => x.id);
                    table.ForeignKey(
                        name: "FK_pages_asp_net_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_pages_asp_net_users_deleted_by_id",
                        column: x => x.deleted_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_pages_asp_net_users_modified_by_id",
                        column: x => x.modified_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_pages_asp_net_users_published_by_id",
                        column: x => x.published_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_pages_pages_original_page_id",
                        column: x => x.original_page_id,
                        principalTable: "pages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "people",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tmdb_id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    profile_path = table.Column<string>(type: "text", nullable: true),
                    gender = table.Column<int>(type: "integer", nullable: true),
                    biography = table.Column<string>(type: "text", nullable: true),
                    birthday = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deathday = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    place_of_birth = table.Column<string>(type: "text", nullable: true),
                    popularity = table.Column<decimal>(type: "numeric", nullable: true),
                    known_for_department = table.Column<string>(type: "text", nullable: true),
                    also_known_as = table.Column<List<string>>(type: "text[]", nullable: true),
                    homepage = table.Column<string>(type: "text", nullable: true),
                    last_updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
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
                    table.PrimaryKey("PK_people", x => x.id);
                    table.ForeignKey(
                        name: "FK_people_asp_net_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_people_asp_net_users_deleted_by_id",
                        column: x => x.deleted_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_people_asp_net_users_modified_by_id",
                        column: x => x.modified_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "performance_metrics",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    path = table.Column<string>(type: "text", nullable: false),
                    load_time = table.Column<double>(type: "double precision", nullable: false),
                    first_contentful_paint = table.Column<double>(type: "double precision", nullable: false),
                    largest_contentful_paint = table.Column<double>(type: "double precision", nullable: false),
                    first_input_delay = table.Column<double>(type: "double precision", nullable: false),
                    cumulative_layout_shift = table.Column<double>(type: "double precision", nullable: false),
                    user_agent = table.Column<string>(type: "text", nullable: true),
                    device_type = table.Column<string>(type: "text", nullable: true),
                    network_info = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("PK_performance_metrics", x => x.id);
                    table.ForeignKey(
                        name: "FK_performance_metrics_asp_net_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_performance_metrics_asp_net_users_deleted_by_id",
                        column: x => x.deleted_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_performance_metrics_asp_net_users_modified_by_id",
                        column: x => x.modified_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TvShows",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    OriginalName = table.Column<string>(type: "text", nullable: true),
                    FirstAirDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastAirDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<string>(type: "text", nullable: true),
                    NumberOfSeasons = table.Column<int>(type: "integer", nullable: true),
                    NumberOfEpisodes = table.Column<int>(type: "integer", nullable: true),
                    EpisodeRunTime = table.Column<int>(type: "integer", nullable: true),
                    Networks = table.Column<string>(type: "text", nullable: true),
                    InProduction = table.Column<bool>(type: "boolean", nullable: true),
                    GenresString = table.Column<string>(type: "text", nullable: true),
                    OriginCountry = table.Column<string>(type: "text", nullable: true),
                    OriginalLanguage = table.Column<string>(type: "text", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedById = table.Column<string>(type: "text", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifiedById = table.Column<string>(type: "text", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedById = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    TmdbId = table.Column<int>(type: "integer", nullable: false),
                    Slug = table.Column<string>(type: "text", nullable: true),
                    Overview = table.Column<string>(type: "text", nullable: true),
                    PosterPath = table.Column<string>(type: "text", nullable: true),
                    BackdropPath = table.Column<string>(type: "text", nullable: true),
                    VoteAverage = table.Column<decimal>(type: "numeric", nullable: true),
                    VoteCount = table.Column<int>(type: "integer", nullable: true),
                    Popularity = table.Column<decimal>(type: "numeric", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TvShows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TvShows_asp_net_users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "asp_net_users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_TvShows_asp_net_users_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "asp_net_users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_TvShows_asp_net_users_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "asp_net_users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_asp_net_users_UserId",
                        column: x => x.UserId,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ab_test_metric",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    variant = table.Column<string>(type: "text", nullable: false),
                    metric_name = table.Column<string>(type: "text", nullable: false),
                    value = table.Column<double>(type: "double precision", nullable: false),
                    sample_size = table.Column<int>(type: "integer", nullable: false),
                    ab_test_id = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_ab_test_metric", x => x.id);
                    table.ForeignKey(
                        name: "FK_ab_test_metric_ab_tests_ab_test_id",
                        column: x => x.ab_test_id,
                        principalTable: "ab_tests",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ab_test_metric_asp_net_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ab_test_metric_asp_net_users_deleted_by_id",
                        column: x => x.deleted_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ab_test_metric_asp_net_users_modified_by_id",
                        column: x => x.modified_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "movie_collections",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    movie_id = table.Column<Guid>(type: "uuid", nullable: false),
                    collection_id = table.Column<Guid>(type: "uuid", nullable: false),
                    order = table.Column<int>(type: "integer", nullable: false),
                    notes = table.Column<string>(type: "text", nullable: true),
                    added_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
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
                    table.PrimaryKey("PK_movie_collections", x => x.id);
                    table.ForeignKey(
                        name: "FK_movie_collections_Movies_movie_id",
                        column: x => x.movie_id,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_movie_collections_asp_net_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_movie_collections_asp_net_users_deleted_by_id",
                        column: x => x.deleted_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_movie_collections_asp_net_users_modified_by_id",
                        column: x => x.modified_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_movie_collections_collections_collection_id",
                        column: x => x.collection_id,
                        principalTable: "collections",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "movie_cast",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    movie_id = table.Column<Guid>(type: "uuid", nullable: false),
                    person_id = table.Column<Guid>(type: "uuid", nullable: false),
                    character = table.Column<string>(type: "text", nullable: true),
                    order = table.Column<int>(type: "integer", nullable: false),
                    credit_id = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("PK_movie_cast", x => x.id);
                    table.ForeignKey(
                        name: "FK_movie_cast_Movies_movie_id",
                        column: x => x.movie_id,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_movie_cast_asp_net_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_movie_cast_asp_net_users_deleted_by_id",
                        column: x => x.deleted_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_movie_cast_asp_net_users_modified_by_id",
                        column: x => x.modified_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_movie_cast_people_person_id",
                        column: x => x.person_id,
                        principalTable: "people",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "movie_crew",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    movie_id = table.Column<Guid>(type: "uuid", nullable: false),
                    person_id = table.Column<Guid>(type: "uuid", nullable: false),
                    department = table.Column<string>(type: "text", nullable: true),
                    job = table.Column<string>(type: "text", nullable: true),
                    credit_id = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("PK_movie_crew", x => x.id);
                    table.ForeignKey(
                        name: "FK_movie_crew_Movies_movie_id",
                        column: x => x.movie_id,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_movie_crew_asp_net_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_movie_crew_asp_net_users_deleted_by_id",
                        column: x => x.deleted_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_movie_crew_asp_net_users_modified_by_id",
                        column: x => x.modified_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_movie_crew_people_person_id",
                        column: x => x.person_id,
                        principalTable: "people",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "media_genres",
                columns: table => new
                {
                    media_id = table.Column<Guid>(type: "uuid", nullable: false),
                    genre_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_media_genres", x => new { x.media_id, x.genre_id });
                    table.ForeignKey(
                        name: "FK_media_genres_Movies_media_id",
                        column: x => x.media_id,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_media_genres_TvShows_media_id",
                        column: x => x.media_id,
                        principalTable: "TvShows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_media_genres_genres_genre_id",
                        column: x => x.genre_id,
                        principalTable: "genres",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "media_translation",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    media_id = table.Column<Guid>(type: "uuid", nullable: false),
                    language_code = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    overview = table.Column<string>(type: "text", nullable: true),
                    slug = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    MovieId = table.Column<Guid>(type: "uuid", nullable: true),
                    TvShowId = table.Column<Guid>(type: "uuid", nullable: true),
                    language_id = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_media_translation", x => x.id);
                    table.ForeignKey(
                        name: "FK_media_translation_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_media_translation_TvShows_TvShowId",
                        column: x => x.TvShowId,
                        principalTable: "TvShows",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_media_translation_asp_net_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_media_translation_asp_net_users_deleted_by_id",
                        column: x => x.deleted_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_media_translation_asp_net_users_modified_by_id",
                        column: x => x.modified_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_media_translation_languages_language_id",
                        column: x => x.language_id,
                        principalTable: "languages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MediaGenre",
                columns: table => new
                {
                    MediaId = table.Column<Guid>(type: "uuid", nullable: false),
                    GenreId = table.Column<Guid>(type: "uuid", nullable: false),
                    MovieId = table.Column<Guid>(type: "uuid", nullable: true),
                    TvShowId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaGenre", x => x.MediaId);
                    table.ForeignKey(
                        name: "FK_MediaGenre_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MediaGenre_TvShows_TvShowId",
                        column: x => x.TvShowId,
                        principalTable: "TvShows",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MediaGenre_genres_GenreId",
                        column: x => x.GenreId,
                        principalTable: "genres",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "seasons",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tmdb_id = table.Column<int>(type: "integer", nullable: false),
                    tv_show_id = table.Column<Guid>(type: "uuid", nullable: false),
                    season_number = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "text", nullable: true),
                    overview = table.Column<string>(type: "text", nullable: true),
                    poster_path = table.Column<string>(type: "text", nullable: true),
                    air_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    episode_count = table.Column<int>(type: "integer", nullable: true),
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
                    table.PrimaryKey("PK_seasons", x => x.id);
                    table.ForeignKey(
                        name: "FK_seasons_TvShows_tv_show_id",
                        column: x => x.tv_show_id,
                        principalTable: "TvShows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_seasons_asp_net_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_seasons_asp_net_users_deleted_by_id",
                        column: x => x.deleted_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_seasons_asp_net_users_modified_by_id",
                        column: x => x.modified_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tv_show_cast",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tv_show_id = table.Column<Guid>(type: "uuid", nullable: false),
                    person_id = table.Column<Guid>(type: "uuid", nullable: false),
                    character = table.Column<string>(type: "text", nullable: true),
                    order = table.Column<int>(type: "integer", nullable: false),
                    credit_id = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("PK_tv_show_cast", x => x.id);
                    table.ForeignKey(
                        name: "FK_tv_show_cast_TvShows_tv_show_id",
                        column: x => x.tv_show_id,
                        principalTable: "TvShows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tv_show_cast_asp_net_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tv_show_cast_asp_net_users_deleted_by_id",
                        column: x => x.deleted_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tv_show_cast_asp_net_users_modified_by_id",
                        column: x => x.modified_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tv_show_cast_people_person_id",
                        column: x => x.person_id,
                        principalTable: "people",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tv_show_crew",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tv_show_id = table.Column<Guid>(type: "uuid", nullable: false),
                    person_id = table.Column<Guid>(type: "uuid", nullable: false),
                    department = table.Column<string>(type: "text", nullable: true),
                    job = table.Column<string>(type: "text", nullable: true),
                    credit_id = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("PK_tv_show_crew", x => x.id);
                    table.ForeignKey(
                        name: "FK_tv_show_crew_TvShows_tv_show_id",
                        column: x => x.tv_show_id,
                        principalTable: "TvShows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tv_show_crew_asp_net_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tv_show_crew_asp_net_users_deleted_by_id",
                        column: x => x.deleted_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tv_show_crew_asp_net_users_modified_by_id",
                        column: x => x.modified_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tv_show_crew_people_person_id",
                        column: x => x.person_id,
                        principalTable: "people",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "episodes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tmdb_id = table.Column<int>(type: "integer", nullable: false),
                    season_id = table.Column<Guid>(type: "uuid", nullable: false),
                    episode_number = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    overview = table.Column<string>(type: "text", nullable: true),
                    still_path = table.Column<string>(type: "text", nullable: true),
                    air_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    runtime = table.Column<int>(type: "integer", nullable: true),
                    vote_average = table.Column<decimal>(type: "numeric", nullable: true),
                    vote_count = table.Column<int>(type: "integer", nullable: true),
                    crew = table.Column<string>(type: "text", nullable: true),
                    guest_stars = table.Column<string>(type: "text", nullable: true),
                    production_code = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("PK_episodes", x => x.id);
                    table.ForeignKey(
                        name: "FK_episodes_asp_net_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_episodes_asp_net_users_deleted_by_id",
                        column: x => x.deleted_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_episodes_asp_net_users_modified_by_id",
                        column: x => x.modified_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_episodes_seasons_season_id",
                        column: x => x.season_id,
                        principalTable: "seasons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "downloads",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    language = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    quality = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    media_type = table.Column<int>(type: "integer", nullable: false),
                    movie_id = table.Column<Guid>(type: "uuid", nullable: true),
                    tv_show_id = table.Column<Guid>(type: "uuid", nullable: true),
                    season_id = table.Column<Guid>(type: "uuid", nullable: true),
                    episode_id = table.Column<Guid>(type: "uuid", nullable: true),
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
                    table.PrimaryKey("PK_downloads", x => x.id);
                    table.CheckConstraint("CK_Download_SingleMediaType", "((CASE WHEN movie_id IS NOT NULL THEN 1 ELSE 0 END) + (CASE WHEN tv_show_id IS NOT NULL THEN 1 ELSE 0 END) + (CASE WHEN season_id IS NOT NULL THEN 1 ELSE 0 END) + (CASE WHEN episode_id IS NOT NULL THEN 1 ELSE 0 END)) = 1");
                    table.ForeignKey(
                        name: "FK_downloads_Movies_movie_id",
                        column: x => x.movie_id,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_downloads_TvShows_tv_show_id",
                        column: x => x.tv_show_id,
                        principalTable: "TvShows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_downloads_asp_net_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_downloads_asp_net_users_deleted_by_id",
                        column: x => x.deleted_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_downloads_asp_net_users_modified_by_id",
                        column: x => x.modified_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_downloads_episodes_episode_id",
                        column: x => x.episode_id,
                        principalTable: "episodes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_downloads_seasons_season_id",
                        column: x => x.season_id,
                        principalTable: "seasons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ab_test_metric_ab_test_id",
                table: "ab_test_metric",
                column: "ab_test_id");

            migrationBuilder.CreateIndex(
                name: "IX_ab_test_metric_created_by_id",
                table: "ab_test_metric",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_ab_test_metric_deleted_by_id",
                table: "ab_test_metric",
                column: "deleted_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_ab_test_metric_modified_by_id",
                table: "ab_test_metric",
                column: "modified_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_ab_tests_created_by_id",
                table: "ab_tests",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_ab_tests_deleted_by_id",
                table: "ab_tests",
                column: "deleted_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_ab_tests_modified_by_id",
                table: "ab_tests",
                column: "modified_by_id");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "asp_net_users",
                column: "normalized_email");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "asp_net_users",
                column: "normalized_user_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_collections_created_by_id",
                table: "collections",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_collections_deleted_by_id",
                table: "collections",
                column: "deleted_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_collections_modified_by_id",
                table: "collections",
                column: "modified_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_collections_owner_id",
                table: "collections",
                column: "owner_id");

            migrationBuilder.CreateIndex(
                name: "IX_downloads_created_by_id",
                table: "downloads",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_downloads_deleted_by_id",
                table: "downloads",
                column: "deleted_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_downloads_episode_id_language_quality",
                table: "downloads",
                columns: new[] { "episode_id", "language", "quality" },
                filter: "episode_id IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_downloads_language",
                table: "downloads",
                column: "language");

            migrationBuilder.CreateIndex(
                name: "IX_downloads_media_type",
                table: "downloads",
                column: "media_type");

            migrationBuilder.CreateIndex(
                name: "IX_downloads_modified_by_id",
                table: "downloads",
                column: "modified_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_downloads_movie_id_language_quality",
                table: "downloads",
                columns: new[] { "movie_id", "language", "quality" },
                filter: "movie_id IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_downloads_quality",
                table: "downloads",
                column: "quality");

            migrationBuilder.CreateIndex(
                name: "IX_downloads_season_id_language_quality",
                table: "downloads",
                columns: new[] { "season_id", "language", "quality" },
                filter: "season_id IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_downloads_tv_show_id_language_quality",
                table: "downloads",
                columns: new[] { "tv_show_id", "language", "quality" },
                filter: "tv_show_id IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_episodes_created_by_id",
                table: "episodes",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_episodes_deleted_by_id",
                table: "episodes",
                column: "deleted_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_episodes_modified_by_id",
                table: "episodes",
                column: "modified_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_episodes_season_id_episode_number",
                table: "episodes",
                columns: new[] { "season_id", "episode_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_episodes_tmdb_id",
                table: "episodes",
                column: "tmdb_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_feature_flags_created_by_id",
                table: "feature_flags",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_feature_flags_deleted_by_id",
                table: "feature_flags",
                column: "deleted_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_feature_flags_modified_by_id",
                table: "feature_flags",
                column: "modified_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_genres_created_by_id",
                table: "genres",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_genres_deleted_by_id",
                table: "genres",
                column: "deleted_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_genres_modified_by_id",
                table: "genres",
                column: "modified_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_genres_tmdb_id",
                table: "genres",
                column: "tmdb_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_jobs_created_by_id",
                table: "jobs",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_jobs_deleted_by_id",
                table: "jobs",
                column: "deleted_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_jobs_modified_by_id",
                table: "jobs",
                column: "modified_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_languages_created_by_id",
                table: "languages",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_languages_deleted_by_id",
                table: "languages",
                column: "deleted_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_languages_iso_code",
                table: "languages",
                column: "iso_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_languages_modified_by_id",
                table: "languages",
                column: "modified_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_media_genres_genre_id",
                table: "media_genres",
                column: "genre_id");

            migrationBuilder.CreateIndex(
                name: "IX_media_translation_created_by_id",
                table: "media_translation",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_media_translation_deleted_by_id",
                table: "media_translation",
                column: "deleted_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_media_translation_language_id",
                table: "media_translation",
                column: "language_id");

            migrationBuilder.CreateIndex(
                name: "IX_media_translation_modified_by_id",
                table: "media_translation",
                column: "modified_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_media_translation_MovieId",
                table: "media_translation",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_media_translation_TvShowId",
                table: "media_translation",
                column: "TvShowId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaGenre_GenreId",
                table: "MediaGenre",
                column: "GenreId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaGenre_MovieId",
                table: "MediaGenre",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaGenre_TvShowId",
                table: "MediaGenre",
                column: "TvShowId");

            migrationBuilder.CreateIndex(
                name: "IX_movie_cast_created_by_id",
                table: "movie_cast",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_movie_cast_deleted_by_id",
                table: "movie_cast",
                column: "deleted_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_movie_cast_modified_by_id",
                table: "movie_cast",
                column: "modified_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_movie_cast_movie_id_person_id",
                table: "movie_cast",
                columns: new[] { "movie_id", "person_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_movie_cast_person_id",
                table: "movie_cast",
                column: "person_id");

            migrationBuilder.CreateIndex(
                name: "IX_MovieCast_CreditId_NonNull",
                table: "movie_cast",
                column: "credit_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_movie_collections_collection_id_movie_id",
                table: "movie_collections",
                columns: new[] { "collection_id", "movie_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_movie_collections_created_by_id",
                table: "movie_collections",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_movie_collections_deleted_by_id",
                table: "movie_collections",
                column: "deleted_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_movie_collections_modified_by_id",
                table: "movie_collections",
                column: "modified_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_movie_collections_movie_id",
                table: "movie_collections",
                column: "movie_id");

            migrationBuilder.CreateIndex(
                name: "IX_movie_crew_created_by_id",
                table: "movie_crew",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_movie_crew_deleted_by_id",
                table: "movie_crew",
                column: "deleted_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_movie_crew_modified_by_id",
                table: "movie_crew",
                column: "modified_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_movie_crew_person_id",
                table: "movie_crew",
                column: "person_id");

            migrationBuilder.CreateIndex(
                name: "IX_MovieCrew_CreditId_NonNull",
                table: "movie_crew",
                column: "credit_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MovieCrew_MovieId_PersonId_Job_NonNull",
                table: "movie_crew",
                columns: new[] { "movie_id", "person_id", "job" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Movies_CreatedById",
                table: "Movies",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_DeletedById",
                table: "Movies",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_ModifiedById",
                table: "Movies",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_Slug_NonNull",
                table: "Movies",
                column: "Slug");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_TmdbId",
                table: "Movies",
                column: "TmdbId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_page_views_created_by_id",
                table: "page_views",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_page_views_deleted_by_id",
                table: "page_views",
                column: "deleted_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_page_views_modified_by_id",
                table: "page_views",
                column: "modified_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_pages_created_by_id",
                table: "pages",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_pages_deleted_by_id",
                table: "pages",
                column: "deleted_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_Pages_LinkTarget_NonDeleted",
                table: "pages",
                column: "link_target",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_pages_modified_by_id",
                table: "pages",
                column: "modified_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_Pages_OriginalPageId_IsPublished_Unique",
                table: "pages",
                columns: new[] { "original_page_id", "is_published" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_pages_published_by_id",
                table: "pages",
                column: "published_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_Pages_Slug_NonDeleted",
                table: "pages",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_pages_status",
                table: "pages",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_people_created_by_id",
                table: "people",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_people_deleted_by_id",
                table: "people",
                column: "deleted_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_people_modified_by_id",
                table: "people",
                column: "modified_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_people_tmdb_id",
                table: "people",
                column: "tmdb_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_performance_metrics_created_by_id",
                table: "performance_metrics",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_performance_metrics_deleted_by_id",
                table: "performance_metrics",
                column: "deleted_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_performance_metrics_modified_by_id",
                table: "performance_metrics",
                column: "modified_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_seasons_created_by_id",
                table: "seasons",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_seasons_deleted_by_id",
                table: "seasons",
                column: "deleted_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_seasons_modified_by_id",
                table: "seasons",
                column: "modified_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_seasons_tmdb_id",
                table: "seasons",
                column: "tmdb_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_seasons_tv_show_id_season_number",
                table: "seasons",
                columns: new[] { "tv_show_id", "season_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tv_show_cast_created_by_id",
                table: "tv_show_cast",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_tv_show_cast_deleted_by_id",
                table: "tv_show_cast",
                column: "deleted_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_tv_show_cast_modified_by_id",
                table: "tv_show_cast",
                column: "modified_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_tv_show_cast_person_id",
                table: "tv_show_cast",
                column: "person_id");

            migrationBuilder.CreateIndex(
                name: "IX_tv_show_cast_tv_show_id_person_id",
                table: "tv_show_cast",
                columns: new[] { "tv_show_id", "person_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TvShowCast_CreditId_NonNull",
                table: "tv_show_cast",
                column: "credit_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tv_show_crew_created_by_id",
                table: "tv_show_crew",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_tv_show_crew_deleted_by_id",
                table: "tv_show_crew",
                column: "deleted_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_tv_show_crew_modified_by_id",
                table: "tv_show_crew",
                column: "modified_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_tv_show_crew_person_id",
                table: "tv_show_crew",
                column: "person_id");

            migrationBuilder.CreateIndex(
                name: "IX_TvShowCrew_CreditId_NonNull",
                table: "tv_show_crew",
                column: "credit_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TvShowCrew_TvShowId_PersonId_Job_NonNull",
                table: "tv_show_crew",
                columns: new[] { "tv_show_id", "person_id", "job" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TvShows_CreatedById",
                table: "TvShows",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TvShows_DeletedById",
                table: "TvShows",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_TvShows_ModifiedById",
                table: "TvShows",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_TvShows_Slug_NonNull",
                table: "TvShows",
                column: "Slug");

            migrationBuilder.CreateIndex(
                name: "IX_TvShows_TmdbId",
                table: "TvShows",
                column: "TmdbId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ab_test_metric");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "downloads");

            migrationBuilder.DropTable(
                name: "feature_flags");

            migrationBuilder.DropTable(
                name: "jobs");

            migrationBuilder.DropTable(
                name: "media_genres");

            migrationBuilder.DropTable(
                name: "media_translation");

            migrationBuilder.DropTable(
                name: "MediaGenre");

            migrationBuilder.DropTable(
                name: "movie_cast");

            migrationBuilder.DropTable(
                name: "movie_collections");

            migrationBuilder.DropTable(
                name: "movie_crew");

            migrationBuilder.DropTable(
                name: "page_views");

            migrationBuilder.DropTable(
                name: "pages");

            migrationBuilder.DropTable(
                name: "performance_metrics");

            migrationBuilder.DropTable(
                name: "tv_show_cast");

            migrationBuilder.DropTable(
                name: "tv_show_crew");

            migrationBuilder.DropTable(
                name: "ab_tests");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "episodes");

            migrationBuilder.DropTable(
                name: "languages");

            migrationBuilder.DropTable(
                name: "genres");

            migrationBuilder.DropTable(
                name: "collections");

            migrationBuilder.DropTable(
                name: "Movies");

            migrationBuilder.DropTable(
                name: "people");

            migrationBuilder.DropTable(
                name: "seasons");

            migrationBuilder.DropTable(
                name: "TvShows");

            migrationBuilder.DropTable(
                name: "asp_net_users");
        }
    }
}
