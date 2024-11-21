using Microsoft.EntityFrameworkCore;
using ProductManagement.Core.Entities;
using ProductManagement.Core.Interfaces;
using ProductManagement.Core.Models;
using ProductManagement.Infrastructure.Data;
using System.Data;

namespace ProductManagement.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
        => await _context.Products.ToListAsync();

    public async Task<Product?> GetByIdAsync(int id)
        => await _context.Products.FindAsync(id);

    public async Task<Product> CreateAsync(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task UpdateAsync(Product product)
    {
        _context.Entry(product).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product != null)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
    }

    //public async Task<Dictionary<string, decimal>> GetCategoryAveragesAsync()
    //{
    //    var averages = await _context.GetCategoryAveragesAsync();
    //    return averages.ToDictionary(
    //        x => x.Category,
    //        x => x.AveragePrice
    //    );
    //}

    //public async Task<Dictionary<string, decimal>> GetCategoryAveragesAsync()
    //{
    //    return await _context.Products
    //        .GroupBy(p => p.Category)
    //        .Select(g => new { Category = g.Key, Average = g.Average(p => p.Price) })
    //        .ToDictionaryAsync(x => x.Category, x => x.Average);
    //}


    //public async Task<Dictionary<string, decimal>> GetCategoryAveragesAsync()
    //{
    //    // Remove EXEC from the stored procedure call
    //    var results = await _context.Database
    //        .SqlQuery<CategoryAverage>($"CalculateCategoryAverages")
    //        .ToListAsync();

    //    return results.ToDictionary(x => x.Category, x => x.AveragePrice);
    //}

    public async Task<Dictionary<string, decimal>> GetCategoryAveragesAsync()
    {
        // For testing environment (in-memory database)
        if (_context.Database.ProviderName?.Contains("InMemory") == true)
        {
            return await _context.Products
                .GroupBy(p => p.Category)
                .Select(g => new
                {
                    Category = g.Key,
                    AveragePrice = g.Average(p => p.Price)
                })
                .ToDictionaryAsync(
                    x => x.Category,
                    x => Math.Round(x.AveragePrice, 2)
                );
        }

        // For production environment (SQL Server)
        using (var command = _context.Database.GetDbConnection().CreateCommand())
        {
            command.CommandText = "CalculateCategoryAverages";
            command.CommandType = System.Data.CommandType.StoredProcedure;

            if (command.Connection!.State != System.Data.ConnectionState.Open)
            {
                await command.Connection.OpenAsync();
            }

            var result = new Dictionary<string, decimal>();
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var category = reader.GetString(0);
                    var average = reader.GetDecimal(1);
                    result.Add(category, Math.Round(average, 2));
                }
            }
            return result;
        }
    }

    //public async Task<string> GetHighestStockValueCategoryAsync()
    //{
    //    // Method 1: Using ADO.NET (Recommended for stored procedures)
    //    using (var command = _context.Database.GetDbConnection().CreateCommand())
    //    {
    //        command.CommandText = "CalculateHighestStockValueCategory";
    //        command.CommandType = CommandType.StoredProcedure;

    //        if (command.Connection!.State != ConnectionState.Open)
    //        {
    //            await command.Connection.OpenAsync();
    //        }

    //        var result = await command.ExecuteScalarAsync();
    //        return result?.ToString() ?? string.Empty;
    //    }

    //}

    public async Task<string> GetHighestStockValueCategoryAsync()
    {
        //  (in-memory database)
        if (_context.Database.ProviderName?.Contains("InMemory") == true)
        {
            return await _context.Products
                .GroupBy(p => p.Category)
                .Select(g => new
                {
                    Category = g.Key,
                    TotalValue = g.Sum(p => p.Price * p.Stock)
                })
                .OrderByDescending(x => x.TotalValue)
                .Select(x => x.Category)
                .FirstOrDefaultAsync() ?? string.Empty;
        }

        //  (SQL Server)
        using (var command = _context.Database.GetDbConnection().CreateCommand())
        {
            command.CommandText = "CalculateHighestStockValueCategory";
            command.CommandType = System.Data.CommandType.StoredProcedure;

            if (command.Connection!.State != System.Data.ConnectionState.Open)
            {
                await command.Connection.OpenAsync();
            }

            var result = await command.ExecuteScalarAsync();
            return result?.ToString() ?? string.Empty;
        }
    }


    //public async Task<string> GetHighestStockValueCategoryAsync()
    //{
    //    return await _context.Products
    //        .GroupBy(p => p.Category)
    //        .Select(g => new
    //        {
    //            Category = g.Key,
    //            TotalValue = g.Sum(p => p.Price * p.Stock)
    //        })
    //        .OrderByDescending(x => x.TotalValue)
    //        .Select(x => x.Category)
    //        .FirstAsync();
    //}
}