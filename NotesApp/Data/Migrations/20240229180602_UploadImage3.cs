using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NotesApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class UploadImage3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfilePicture",
                table: "Note");

            migrationBuilder.AddColumn<string>(
                name: "ImageFileName",
                table: "Note",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageFileName",
                table: "Note");

            migrationBuilder.AddColumn<string>(
                name: "ProfilePicture",
                table: "Note",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
