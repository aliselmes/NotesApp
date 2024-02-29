using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NotesApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class userID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OwnerID",
                table: "Note",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OwnerID",
                table: "Note");
        }
    }
}
