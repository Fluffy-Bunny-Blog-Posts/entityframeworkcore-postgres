using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApp.Migrations.AppEntityCore
{
    public partial class Initial2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Counties_States_StateFK",
                table: "Counties");

            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    Updated = table.Column<DateTime>(nullable: false),
                    StateFK = table.Column<string>(nullable: true),
                    CountyFK = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cities_Counties_CountyFK",
                        column: x => x.CountyFK,
                        principalTable: "Counties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cities_CountyFK",
                table: "Cities",
                column: "CountyFK");

            migrationBuilder.AddForeignKey(
                name: "FK_Counties_States_StateFK",
                table: "Counties",
                column: "StateFK",
                principalTable: "States",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Counties_States_StateFK",
                table: "Counties");

            migrationBuilder.DropTable(
                name: "Cities");

            migrationBuilder.AddForeignKey(
                name: "FK_Counties_States_StateFK",
                table: "Counties",
                column: "StateFK",
                principalTable: "States",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
