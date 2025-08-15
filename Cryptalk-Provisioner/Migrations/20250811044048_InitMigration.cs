using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cryptalk_Provisioner.Migrations
{
    /// <inheritdoc />
    public partial class InitMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "allocations",
                columns: table => new
                {
                    install_id = table.Column<string>(type: "text", nullable: false),
                    fqdn = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    ipv4 = table.Column<string>(type: "text", nullable: true),
                    ipv6 = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_allocations", x => x.install_id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_allocations_fqdn",
                table: "allocations",
                column: "fqdn",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "allocations");
        }
    }
}
