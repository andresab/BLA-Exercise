using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TaskManagement.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedSampleData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "id", "email", "name" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "ada@example.com", "Ada Lovelace" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "grace@example.com", "Grace Hopper" }
                });

            migrationBuilder.InsertData(
                table: "tasks",
                columns: new[] { "id", "description", "due_date", "status", "title", "user_id" },
                values: new object[,]
                {
                    { new Guid("33333333-3333-3333-3333-333333333333"), "Review backlog and define priorities.", new DateTime(2026, 7, 1, 14, 0, 0, 0, DateTimeKind.Utc), "Pending", "Prepare sprint planning", new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("44444444-4444-4444-4444-444444444444"), "Send the final version to stakeholders.", new DateTime(2026, 7, 3, 12, 0, 0, 0, DateTimeKind.Utc), "InProgress", "Publish release notes", new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("55555555-5555-5555-5555-555555555555"), "Confirm operational readiness items.", new DateTime(2026, 7, 5, 16, 0, 0, 0, DateTimeKind.Utc), "Pending", "Review production checklist", new Guid("22222222-2222-2222-2222-222222222222") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"));

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));
        }
    }
}
