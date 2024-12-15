using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ustaw_temperature.Data.Migrations
{
    /// <inheritdoc />
    public partial class initional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Mieszkanie",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LiczbaOkien = table.Column<int>(type: "int", nullable: false),
                    LiczbaPokoi = table.Column<int>(type: "int", nullable: false),
                    BazowaTemperatura = table.Column<int>(type: "int", nullable: false),
                    Uzytkownik = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Uzytkownik2 = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mieszkanie", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Mieszkanie_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Harmonogram",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nazwa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Start = table.Column<int>(type: "int", nullable: false),
                    End = table.Column<int>(type: "int", nullable: false),
                    DocelowaTemperatura = table.Column<int>(type: "int", nullable: false),
                    MieszkanieId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Harmonogram", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Harmonogram_Mieszkanie_MieszkanieId",
                        column: x => x.MieszkanieId,
                        principalTable: "Mieszkanie",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Harmonogram_MieszkanieId",
                table: "Harmonogram",
                column: "MieszkanieId");

            migrationBuilder.CreateIndex(
                name: "IX_Mieszkanie_UserId",
                table: "Mieszkanie",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Harmonogram");

            migrationBuilder.DropTable(
                name: "Mieszkanie");
        }
    }
}
