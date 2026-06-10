using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Restaurant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSidesAndRenames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Description", "ImageUrl", "Name" },
                values: new object[] { "Rich, moist chocolate cake layered with silky chocolate ganache.", "/images/menu/chocolate-cake.jpg", "Chocolate Cake" });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Description", "ImageUrl", "Name" },
                values: new object[] { "Chilled bottled mineral water.", "/images/menu/water.jpg", "Water" });

            migrationBuilder.InsertData(
                table: "MenuItems",
                columns: new[] { "Id", "CategoryId", "Description", "ImageUrl", "IsAvailable", "IsDeleted", "Name", "Price" },
                values: new object[,]
                {
                    { 33, 1, "Crispy golden battered onion rings served with a dipping sauce.", "/images/menu/onion-rings.jpg", true, false, "Onion Rings", 5.50m },
                    { 34, 1, "Thick-cut, golden, crispy fries with sea salt.", "/images/menu/fries.jpg", true, false, "Fries", 4.00m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Description", "ImageUrl", "Name" },
                values: new object[] { "Warm dark-chocolate cake with a molten centre, served with vanilla ice cream.", "/images/menu/lava.jpg", "Chocolate Lava Cake" });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Description", "ImageUrl", "Name" },
                values: new object[] { "Chilled sparkling mineral water with lemon.", "/images/menu/sparkling.jpg", "Sparkling Water" });
        }
    }
}
