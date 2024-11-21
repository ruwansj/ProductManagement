using Microsoft.EntityFrameworkCore;
using ProductManagement.Core.Entities;
using ProductManagement.Core.Models;

namespace ProductManagement.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

 
    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

  
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Category).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Price).HasPrecision(18, 2);
            entity.Property(e => e.Stock).IsRequired();
        });
    }

    
    public async Task<IEnumerable<CategoryAverage>> GetCategoryAveragesAsync()
    {
        return await Products
            .GroupBy(p => p.Category)
            .Select(g => new CategoryAverage
            {
                Category = g.Key,
                AveragePrice = g.Average(p => p.Price)
            })
            .ToListAsync();
    }

    public async Task<string> GetHighestStockValueCategoryAsync()
    {
        return await Products
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
}