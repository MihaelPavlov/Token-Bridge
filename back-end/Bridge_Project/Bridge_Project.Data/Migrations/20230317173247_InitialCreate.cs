using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bridge_Project.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BridgeEvents",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PublicKeySender = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JsonData = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EventType = table.Column<int>(type: "int", nullable: false),
                    RequiresClaiming = table.Column<bool>(type: "bit", nullable: false),
                    IsClaimed = table.Column<bool>(type: "bit", nullable: false),
                    BlockNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChainName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClaimedFromId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BridgeEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BridgeEvents_BridgeEvents_ClaimedFromId",
                        column: x => x.ClaimedFromId,
                        principalTable: "BridgeEvents",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BridgeEvents_ClaimedFromId",
                table: "BridgeEvents",
                column: "ClaimedFromId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BridgeEvents");
        }
    }
}
