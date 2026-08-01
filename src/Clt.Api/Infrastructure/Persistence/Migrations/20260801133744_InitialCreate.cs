using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clt.Api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                CREATE TABLE IF NOT EXISTS "Currencies" (
                    "Id" INTEGER NOT NULL CONSTRAINT "PK_Currencies" PRIMARY KEY AUTOINCREMENT,
                    "Code" TEXT NOT NULL,
                    "Name" TEXT NOT NULL,
                    "RateToBase" TEXT NOT NULL
                );

                CREATE TABLE IF NOT EXISTS "Users" (
                    "Id" INTEGER NOT NULL CONSTRAINT "PK_Users" PRIMARY KEY AUTOINCREMENT,
                    "Name" TEXT NOT NULL,
                    "Email" TEXT NOT NULL,
                    "IsActive" INTEGER NOT NULL DEFAULT 1,
                    "PasswordHash" TEXT NOT NULL
                );

                CREATE TABLE IF NOT EXISTS "Addresses" (
                    "Id" INTEGER NOT NULL CONSTRAINT "PK_Addresses" PRIMARY KEY AUTOINCREMENT,
                    "UserId" INTEGER NOT NULL,
                    "Street" TEXT NOT NULL,
                    "City" TEXT NOT NULL,
                    "Country" TEXT NOT NULL,
                    "ZipCode" TEXT NULL,
                    CONSTRAINT "FK_Addresses_Users_UserId"
                        FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
                );

                CREATE INDEX IF NOT EXISTS "IX_Addresses_UserId" ON "Addresses" ("UserId");
                CREATE UNIQUE INDEX IF NOT EXISTS "IX_Currencies_Code" ON "Currencies" ("Code");
                CREATE UNIQUE INDEX IF NOT EXISTS "IX_Users_Email" ON "Users" ("Email");
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "Currencies");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
