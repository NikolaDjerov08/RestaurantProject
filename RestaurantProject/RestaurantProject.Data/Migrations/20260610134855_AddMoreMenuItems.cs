using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Restaurant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMoreMenuItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 1,
                column: "ImageUrl",
                value: "/images/menu/bruschetta.jpg");

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 2,
                column: "ImageUrl",
                value: "/images/menu/caesar.jpg");

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 3,
                column: "ImageUrl",
                value: "/images/menu/salmon.jpg");

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 4,
                column: "ImageUrl",
                value: "/images/menu/ribeye.jpg");

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 5,
                column: "ImageUrl",
                value: "/images/menu/carbonara.jpg");

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 6,
                column: "ImageUrl",
                value: "/images/menu/arrabbiata.jpg");

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 7,
                column: "ImageUrl",
                value: "/images/menu/tiramisu.jpg");

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 8,
                column: "ImageUrl",
                value: "/images/menu/lava.jpg");

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 9,
                column: "ImageUrl",
                value: "/images/menu/red-wine.jpg");

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 10,
                column: "ImageUrl",
                value: "/images/menu/sparkling.jpg");

            migrationBuilder.InsertData(
                table: "MenuItems",
                columns: new[] { "Id", "CategoryId", "Description", "ImageUrl", "IsAvailable", "IsDeleted", "Name", "Price" },
                values: new object[,]
                {
                    { 11, 1, "Oven-baked baguette with garlic butter and a touch of parsley.", "/images/menu/garlic-bread.jpg", true, false, "Garlic Bread", 5.50m },
                    { 12, 1, "Golden, crispy breaded mozzarella served with marinara dip.", "/images/menu/mozzarella-sticks.jpg", true, false, "Mozzarella Sticks", 6.50m },
                    { 13, 1, "Crispy wings tossed in your choice of buffalo or BBQ sauce.", "/images/menu/chicken-wings.jpg", true, false, "Chicken Wings", 8.50m },
                    { 14, 1, "Creamy roasted-tomato soup with basil and a swirl of cream.", "/images/menu/tomato-soup.jpg", true, false, "Tomato Soup", 6.00m },
                    { 15, 2, "Wood-fired pizza with tomato, fresh mozzarella, and basil.", "/images/menu/margherita-pizza.jpg", true, false, "Margherita Pizza", 12.50m },
                    { 16, 2, "Beef patty, cheddar, lettuce, tomato, and house sauce with fries.", "/images/menu/cheeseburger.jpg", true, false, "Cheeseburger", 13.00m },
                    { 17, 2, "Herb-marinated chicken breast with grilled vegetables.", "/images/menu/grilled-chicken.jpg", true, false, "Grilled Chicken", 16.50m },
                    { 18, 2, "Beer-battered cod with thick-cut chips and tartar sauce.", "/images/menu/fish-and-chips.jpg", true, false, "Fish and Chips", 14.50m },
                    { 19, 2, "Slow-cooked pork ribs glazed in smoky barbecue sauce.", "/images/menu/bbq-ribs.jpg", true, false, "BBQ Ribs", 19.50m },
                    { 20, 5, "Layers of pasta, beef ragù, béchamel, and melted cheese.", "/images/menu/lasagna.jpg", true, false, "Lasagna", 14.00m },
                    { 21, 5, "Fettuccine in a creamy parmesan and butter sauce.", "/images/menu/fettuccine-alfredo.jpg", true, false, "Fettuccine Alfredo", 13.00m },
                    { 22, 3, "Creamy New York-style cheesecake with a berry compote.", "/images/menu/cheesecake.jpg", true, false, "Cheesecake", 7.50m },
                    { 23, 3, "Warm spiced apple pie with a flaky crust and vanilla ice cream.", "/images/menu/apple-pie.jpg", true, false, "Apple Pie", 6.50m },
                    { 24, 3, "Vanilla ice cream, chocolate sauce, nuts, and whipped cream.", "/images/menu/ice-cream-sundae.jpg", true, false, "Ice Cream Sundae", 6.00m },
                    { 25, 4, "Classic ice-cold cola served with lemon.", "/images/menu/coca-cola.jpg", true, false, "Coca-Cola", 3.00m },
                    { 26, 4, "Freshly squeezed orange juice.", "/images/menu/orange-juice.jpg", true, false, "Orange Juice", 4.00m },
                    { 27, 4, "Espresso with steamed milk and a thick layer of foam.", "/images/menu/cappuccino.jpg", true, false, "Cappuccino", 4.50m },
                    { 28, 4, "Homemade lemonade with fresh mint.", "/images/menu/lemonade.jpg", true, false, "Lemonade", 3.50m },
                    { 29, 4, "Locally brewed craft lager, served chilled.", "/images/menu/craft-beer.jpg", true, false, "Craft Beer", 6.00m },
                    { 30, 4, "Rum, lime, mint, and soda over crushed ice.", "/images/menu/mojito.jpg", true, false, "Mojito", 8.50m },
                    { 31, 4, "Rich single-shot espresso.", "/images/menu/espresso.jpg", true, false, "Espresso", 3.00m },
                    { 32, 4, "Refreshing iced tea with lemon.", "/images/menu/iced-tea.jpg", true, false, "Iced Tea", 3.50m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 1,
                column: "ImageUrl",
                value: "/images/menu/bruschetta.svg");

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 2,
                column: "ImageUrl",
                value: "/images/menu/caesar.svg");

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 3,
                column: "ImageUrl",
                value: "/images/menu/salmon.svg");

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 4,
                column: "ImageUrl",
                value: "/images/menu/ribeye.svg");

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 5,
                column: "ImageUrl",
                value: "/images/menu/carbonara.svg");

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 6,
                column: "ImageUrl",
                value: "/images/menu/arrabbiata.svg");

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 7,
                column: "ImageUrl",
                value: "/images/menu/tiramisu.svg");

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 8,
                column: "ImageUrl",
                value: "/images/menu/lava.svg");

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 9,
                column: "ImageUrl",
                value: "/images/menu/red-wine.svg");

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 10,
                column: "ImageUrl",
                value: "/images/menu/sparkling.svg");
        }
    }
}
