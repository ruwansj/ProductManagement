using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Stock = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });
            // Drop existing procedures if they exist
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[CalculateCategoryAverages]");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[CalculateHighestStockValueCategory]");

            // Create Category Averages Stored Procedure
            migrationBuilder.Sql(@"
                                CREATE PROCEDURE [dbo].[CalculateCategoryAverages]
                                AS
                                BEGIN
                                    SET NOCOUNT ON;
                
                                    SELECT 
                                        p.Category,
                                        CAST(AVG(CAST(p.Price AS DECIMAL(18,2))) AS DECIMAL(18,2)) as AveragePrice
                                    FROM Products p
                                    GROUP BY p.Category;
                                END
                            ");

            // Create Highest Stock Value Category Stored Procedure
            migrationBuilder.Sql(@"
                                CREATE PROCEDURE [dbo].[CalculateHighestStockValueCategory]
                                AS
                                BEGIN
                                    SET NOCOUNT ON;
                
                                    WITH CategoryValues AS (
                                        SELECT 
                                            p.Category,
                                            SUM(p.Price * p.Stock) as TotalValue
                                        FROM Products p
                                        GROUP BY p.Category
                                    )
                                    SELECT TOP 1 
                                        Category
                                    FROM CategoryValues
                                    ORDER BY TotalValue DESC;
                                END
                            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
            migrationBuilder.Sql(@"DROP PROCEDURE IF EXISTS [dbo].[CalculateCategoryAverages]");
            migrationBuilder.Sql(@"DROP PROCEDURE IF EXISTS [dbo].[CalculateHighestStockValueCategory]");

        }
    }
}
