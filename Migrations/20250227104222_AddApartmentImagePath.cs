using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingRIo.Migrations
{
    /// <inheritdoc />
    public partial class AddApartmentImagePath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "apartments",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "apartments");
        }
    }
}
