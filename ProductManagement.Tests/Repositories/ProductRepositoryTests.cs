using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProductManagement.Core.Entities;
using ProductManagement.Infrastructure.Data;
using ProductManagement.Infrastructure.Repositories;

namespace ProductManagement.Tests.Repositories;

[TestClass]
public class ProductRepositoryTests
{
    private DbContextOptions<ApplicationDbContext> _options = null!;
    private ApplicationDbContext _context = null!;
    private ProductRepository _repository = null!;

    [TestInitialize]
    public void Setup()
    {
        // Create unique in-memory database for testing
        _options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: $"ProductManagementTest_{Guid.NewGuid()}")
            .Options;

        _context = new ApplicationDbContext(_options);
        _repository = new ProductRepository(_context);
    }

    [TestMethod]
    public async Task GetCategoryAveragesAsync_ShouldReturnCorrectAverages()
    {
        // Arrange - Set up specific test data
        await _context.Products.AddRangeAsync(new List<Product>
        {
            new Product
            {
                Name = "Test Product 1",
                Category = "Category 1",
                Price = 99.99m,
                Stock = 100
            },
            new Product
            {
                Name = "Test Product 2",
                Category = "Category 1",
                Price = 149.99m,
                Stock = 50
            },
            new Product
            {
                Name = "Test Product 3",
                Category = "Category 2",
                Price = 199.99m,
                Stock = 75
            }
        });
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetCategoryAveragesAsync();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count); // Should have 2 categories
        Assert.AreEqual(124.99m, result["Category 1"]); // Average of 99.99 and 149.99
        Assert.AreEqual(199.99m, result["Category 2"]); // Single price
    }

    [TestMethod]
    public async Task GetHighestStockValueCategoryAsync_ShouldReturnCorrectCategory()
    {
        // Arrange - Set up specific test data
        await _context.Products.AddRangeAsync(new List<Product>
        {
            new Product
            {
                Name = "Test Product 1",
                Category = "Category 1",
                Price = 100m,    // Total value: 100 * 100 = 10,000
                Stock = 100
            },
            new Product
            {
                Name = "Test Product 2",
                Category = "Category 2",
                Price = 50m,     // Total value: 50 * 150 = 7,500
                Stock = 150
            }
        });
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetHighestStockValueCategoryAsync();

        // Assert
        Assert.AreEqual("Category 1", result); // Category 1 has higher total value
    }

    [TestMethod]
    public async Task GetAllAsync_ShouldReturnAllProducts()
    {
        try
        {
            // Arrange
            var testProducts = new List<Product>
            {
                new Product
                {
                    Name = "Test Product 1",
                    Category = "Category 1",
                    Price = 99.99m,
                    Stock = 100
                },
                new Product
                {
                    Name = "Test Product 2",
                    Category = "Category 2",
                    Price = 149.99m,
                    Stock = 50
                }
            };

            // Clear any existing data
            _context.Products.RemoveRange(_context.Products);
            await _context.SaveChangesAsync();

            // Add test data
            await _context.Products.AddRangeAsync(testProducts);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllAsync();
            var products = result.ToList();

         
            Console.WriteLine($"Number of products in database: {products.Count}");
            foreach (var product in products)
            {
                Console.WriteLine($"Product: {product.Name}, Category: {product.Category}");
            }

            // Assert
            Assert.AreEqual(2, products.Count, "Expected exactly 2 products in the database");
            Assert.AreEqual("Test Product 1", products[0].Name, "First product name mismatch");
            Assert.AreEqual("Test Product 2", products[1].Name, "Second product name mismatch");
        }
        catch (Exception ex)
        {
            Assert.Fail($"Test failed with exception: {ex.Message}");
        }
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
