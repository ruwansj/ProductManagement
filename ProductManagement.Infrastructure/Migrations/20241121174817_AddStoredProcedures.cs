using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ProductManagement.Infrastructure.Migrations
{
    

    public partial class AddStoredProcedures : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[CalculateCategoryAverages]");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[CalculateHighestStockValueCategory]");
        }
    }
}
