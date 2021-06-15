using Microsoft.EntityFrameworkCore.Migrations;

namespace Stock_Manager_With_Search_Functionality.Migrations
{
    public partial class ProductMigration2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Catagory",
                table: "Product",
                newName: "Category");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Category",
                table: "Product",
                newName: "Catagory");
        }
    }
}
