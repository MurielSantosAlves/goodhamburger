using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GoodHamburger.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddDiscountRules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DiscountRules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    DiscountPercentage = table.Column<decimal>(type: "numeric(5,4)", precision: 5, scale: 4, nullable: false),
                    RequiresSandwich = table.Column<bool>(type: "boolean", nullable: false),
                    RequiresSideDish = table.Column<bool>(type: "boolean", nullable: false),
                    RequiresDrink = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscountRules", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "DiscountRules",
                columns: new[] { "Id", "CreatedAt", "Description", "DiscountPercentage", "IsActive", "Name", "Priority", "RequiresDrink", "RequiresSandwich", "RequiresSideDish", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Sanduíche + Batata + Refrigerante", 0.20m, true, "Combo Completo", 1, true, true, true, null },
                    { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Sanduíche + Refrigerante", 0.15m, true, "Combo Bebida", 2, true, true, false, null },
                    { 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Sanduíche + Batata Frita", 0.10m, true, "Combo Batata", 3, false, true, true, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiscountRules");
        }
    }
}
