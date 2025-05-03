using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Contacts.Migrations
{
    /// <inheritdoc />
    public partial class AddSharedContactTableAndForeignkeystomakerelationbetweenContacts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SharedContacts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Contactid = table.Column<int>(type: "int", nullable: false),
                    Userid = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ShareDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CanEdit = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SharedContacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SharedContacts_AspNetUsers_Userid",
                        column: x => x.Userid,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SharedContacts_Contacts_Contactid",
                        column: x => x.Contactid,
                        principalTable: "Contacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SharedContacts_Contactid",
                table: "SharedContacts",
                column: "Contactid");

            migrationBuilder.CreateIndex(
                name: "IX_SharedContacts_Userid",
                table: "SharedContacts",
                column: "Userid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SharedContacts");
        }
    }
}
