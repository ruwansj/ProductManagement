//using Microsoft.Extensions.Caching.Memory;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Moq;
//using ProductManagement.Core.Entities;
//using ProductManagement.Core.Interfaces;
//using ProductManagement.Core.Services;

//namespace ProductManagement.Tests.Services;

//[TestClass]
//public class ProductServiceTests
//{
//    private Mock<IProductRepository> _mockRepo = null!;
//    private IMemoryCache _cache = null!;
//    private ProductService _service = null!;
//    private List<Product> _testProducts = null!;

//    [TestInitialize]
//    public void Setup()
//    {
//        // Initialize mock repository
//        _mockRepo = new Mock<IProductRepository>();

//        // Initialize memory cache
//        _cache = new MemoryCache(new MemoryCacheOptions());

//        // Initialize service
//        _service = new ProductService(_mockRepo.Object, _cache);

//        // Setup test data
//        _testProducts = new List<Product>
//        {
//            new Product
//            {
//                Id = 1,
//                Name = "Test Product 1",
//                Category = "Category 1",
//                Price = 99.99m,
//                Stock = 100
//            },
//            new Product
//            {
//                Id = 2,
//                Name = "Test Product 2",
//                Category = "Category 2",
//                Price = 149.99m,
//                Stock = 50
//            }
//        };
//    }

//    [TestMethod]
//    public async Task GetAllProductsAsync_ShouldReturnAllProducts()
//    {
//        // Arrange
//        _mockRepo.Setup(repo => repo.GetAllAsync())
//            .ReturnsAsync(_testProducts);

//        // Act
//        var result = await _service.GetAllProductsAsync();
//        var productsList = result.ToList();

//        // Assert
//        Assert.IsNotNull(result);
//        Assert.AreEqual(_testProducts.Count, productsList.Count);
//        Assert.AreEqual(_testProducts[0].Name, productsList[0].Name);
//        Assert.AreEqual(_testProducts[1].Name, productsList[1].Name);
//    }

//    [TestMethod]
//    public async Task GetAllProductsAsync_ShouldReturnCachedResults()
//    {
//        // Arrange
//        _mockRepo.Setup(repo => repo.GetAllAsync())
//            .ReturnsAsync(_testProducts);

//        // Act
//        var result1 = await _service.GetAllProductsAsync();
//        var result2 = await _service.GetAllProductsAsync();

//        // Assert
//        _mockRepo.Verify(repo => repo.GetAllAsync(), Times.Once);
//        Assert.IsNotNull(result1);
//        Assert.IsNotNull(result2);
//    }

//    [TestMethod]
//    public async Task GetProductByIdAsync_ShouldReturnCorrectProduct()
//    {
//        // Arrange
//        var testProduct = _testProducts[0];
//        _mockRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
//            .ReturnsAsync(testProduct);

//        // Act
//        var result = await _service.GetProductByIdAsync(1);

//        // Assert
//        Assert.IsNotNull(result);
//        Assert.AreEqual(testProduct.Id, result.Id);
//        Assert.AreEqual(testProduct.Name, result.Name);
//    }

//    [TestMethod]
//    public async Task CreateProductAsync_ShouldCreateAndReturnProduct()
//    {
//        // Arrange
//        var newProduct = new Product
//        {
//            Name = "New Product",
//            Category = "New Category",
//            Price = 199.99m,
//            Stock = 75
//        };

//        _mockRepo.Setup(repo => repo.CreateAsync(It.IsAny<Product>()))
//            .ReturnsAsync((Product p) => { p.Id = 3; return p; });

//        bool delegateInvoked = false;
//        _service.OnProductModified += (p) => delegateInvoked = true;

//        // Act
//        var result = await _service.CreateProductAsync(newProduct);

//        // Assert
//        Assert.IsNotNull(result);
//        Assert.AreEqual(3, result.Id);
//        Assert.AreEqual(newProduct.Name, result.Name);
//        Assert.IsTrue(delegateInvoked);
//        _mockRepo.Verify(repo => repo.CreateAsync(It.IsAny<Product>()), Times.Once);
//    }

//    [TestMethod]
//    public async Task UpdateProductAsync_ShouldUpdateAndInvokeDelegate()
//    {
//        // Arrange
//        var product = _testProducts[0];
//        bool delegateInvoked = false;
//        _service.OnProductModified += (p) => delegateInvoked = true;

//        // Act
//        await _service.UpdateProductAsync(product);

//        // Assert
//        Assert.IsTrue(delegateInvoked);
//        _mockRepo.Verify(repo => repo.UpdateAsync(It.IsAny<Product>()), Times.Once);
//    }

//    [TestMethod]
//    public async Task DeleteProductAsync_ShouldDeleteAndInvokeDelegate()
//    {
//        // Arrange
//        bool delegateInvoked = false;
//        _service.OnProductModified += (p) => delegateInvoked = true;

//        // Act
//        await _service.DeleteProductAsync(1);

//        // Assert
//        Assert.IsTrue(delegateInvoked);
//        _mockRepo.Verify(repo => repo.DeleteAsync(1), Times.Once);
//    }

//    [TestMethod]
//    public async Task GetCategoryAnalyticsAsync_ShouldReturnCategoryAverages()
//    {
//        // Arrange
//        var categoryAverages = new Dictionary<string, decimal>
//        {
//            { "Category 1", 99.99m },
//            { "Category 2", 149.99m }
//        };

//        _mockRepo.Setup(repo => repo.GetCategoryAveragesAsync())
//            .ReturnsAsync(categoryAverages);

//        // Act
//        var result = await _service.GetCategoryAnalyticsAsync();

//        // Assert
//        Assert.IsNotNull(result);
//        Assert.AreEqual(categoryAverages.Count, result.Count);
//        Assert.AreEqual(categoryAverages["Category 1"], result["Category 1"]);
//        Assert.AreEqual(categoryAverages["Category 2"], result["Category 2"]);
//    }

//    [TestMethod]
//    public async Task GetCategoryAnalyticsAsync_ShouldReturnCachedResults()
//    {
//        // Arrange
//        var categoryAverages = new Dictionary<string, decimal>
//        {
//            { "Category 1", 99.99m },
//            { "Category 2", 149.99m }
//        };

//        _mockRepo.Setup(repo => repo.GetCategoryAveragesAsync())
//            .ReturnsAsync(categoryAverages);

//        // Act
//        var result1 = await _service.GetCategoryAnalyticsAsync();
//        var result2 = await _service.GetCategoryAnalyticsAsync();

//        // Assert
//        _mockRepo.Verify(repo => repo.GetCategoryAveragesAsync(), Times.Once);
//        Assert.IsNotNull(result1);
//        Assert.IsNotNull(result2);
//    }

//    [TestMethod]
//    public async Task GetHighestStockValueCategoryAsync_ShouldReturnCorrectCategory()
//    {
//        // Arrange
//        var expectedCategory = "Category 1";
//        _mockRepo.Setup(repo => repo.GetHighestStockValueCategoryAsync())
//            .ReturnsAsync(expectedCategory);

//        // Act
//        var result = await _service.GetHighestStockValueCategoryAsync();

//        // Assert
//        Assert.AreEqual(expectedCategory, result);
//        _mockRepo.Verify(repo => repo.GetHighestStockValueCategoryAsync(), Times.Once);
//    }
//}

using Microsoft.Extensions.Caching.Memory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ProductManagement.Core.Entities;
using ProductManagement.Core.Interfaces;
using ProductManagement.Core.Services;

namespace ProductManagement.Tests.Services;

[TestClass]
public class ProductServiceTests
{
    private Mock<IProductRepository> _mockRepo = null!;
    private IMemoryCache _cache = null!;
    private ProductService _service = null!;
    private List<Product> _testProducts = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockRepo = new Mock<IProductRepository>();
        _cache = new MemoryCache(new MemoryCacheOptions());
        _service = new ProductService(_mockRepo.Object, _cache);

        _testProducts = new List<Product>
        {
            new Product
            {
                Id = 1,
                Name = "Test Product 1",
                Category = "Category 1",
                Price = 99.99m,
                Stock = 100
            },
            new Product
            {
                Id = 2,
                Name = "Test Product 2",
                Category = "Category 2",
                Price = 149.99m,
                Stock = 50
            }
        };
    }

    [TestMethod]
    public async Task CreateProductAsync_ShouldCreateAndInvokeDelegates()
    {
        // Arrange
        var newProduct = new Product
        {
            Name = "New Product",
            Category = "New Category",
            Price = 199.99m,
            Stock = 75
        };

        _mockRepo.Setup(repo => repo.CreateAsync(It.IsAny<Product>()))
            .ReturnsAsync((Product p) => { p.Id = 3; return p; });

        bool createdDelegateInvoked = false;
        bool priceLowDelegateInvoked = false;

        _service.OnProductCreated += (p) => createdDelegateInvoked = true;
        _service.OnProductPriceLow += (p, t) => priceLowDelegateInvoked = true;
        _service.SetPriceThreshold(200m); // Set threshold above product price

        // Act
        var result = await _service.CreateProductAsync(newProduct);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(3, result.Id);
        Assert.AreEqual(newProduct.Name, result.Name);
        Assert.IsTrue(createdDelegateInvoked, "Created delegate should be invoked");
        Assert.IsTrue(priceLowDelegateInvoked, "Price low delegate should be invoked");
        _mockRepo.Verify(repo => repo.CreateAsync(It.IsAny<Product>()), Times.Once);
    }

    [TestMethod]
    public async Task UpdateProductAsync_ShouldUpdateAndInvokeDelegates()
    {
        // Arrange
        var product = _testProducts[0];
        bool updatedDelegateInvoked = false;
        bool priceLowDelegateInvoked = false;

        _service.OnProductUpdated += (p) => updatedDelegateInvoked = true;
        _service.OnProductPriceLow += (p, t) => priceLowDelegateInvoked = true;
        _service.SetPriceThreshold(100m);

        // Act
        await _service.UpdateProductAsync(product);

        // Assert
        Assert.IsTrue(updatedDelegateInvoked, "Updated delegate should be invoked");
        Assert.IsTrue(priceLowDelegateInvoked, "Price low delegate should be invoked");
        _mockRepo.Verify(repo => repo.UpdateAsync(It.IsAny<Product>()), Times.Once);
    }

    [TestMethod]
    public async Task DeleteProductAsync_ShouldDeleteAndInvokeDelegate()
    {
        // Arrange
        bool deletedDelegateInvoked = false;
        _service.OnProductDeleted += (id) => deletedDelegateInvoked = true;

        // Act
        await _service.DeleteProductAsync(1);

        // Assert
        Assert.IsTrue(deletedDelegateInvoked, "Deleted delegate should be invoked");
        _mockRepo.Verify(repo => repo.DeleteAsync(1), Times.Once);
    }

    [TestMethod]
    public void SetPriceThreshold_ShouldUpdateThreshold()
    {
        // Arrange
        var testThreshold = 150.0m;
        bool priceLowDelegateInvoked = false;
        _service.OnProductPriceLow += (p, t) =>
        {
            priceLowDelegateInvoked = true;
            Assert.AreEqual(testThreshold, t);
        };

        // Act
        _service.SetPriceThreshold(testThreshold);
        var product = new Product { Price = 100m };
        _service.CreateProductAsync(product).Wait();

        // Assert
        Assert.IsTrue(priceLowDelegateInvoked, "Price low delegate should be invoked with correct threshold");
    }

    [TestMethod]
    public async Task GetAllProductsAsync_ShouldReturnAllProducts()
    {
        // Arrange
        _mockRepo.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(_testProducts);

        // Act
        var result = await _service.GetAllProductsAsync();
        var productsList = result.ToList();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(_testProducts.Count, productsList.Count);
        Assert.AreEqual(_testProducts[0].Name, productsList[0].Name);
        Assert.AreEqual(_testProducts[1].Name, productsList[1].Name);
    }

    [TestMethod]
    public async Task GetAllProductsAsync_ShouldReturnCachedResults()
    {
        // Arrange
        _mockRepo.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(_testProducts);

        // Act
        var result1 = await _service.GetAllProductsAsync();
        var result2 = await _service.GetAllProductsAsync();

        // Assert
        _mockRepo.Verify(repo => repo.GetAllAsync(), Times.Once);
        Assert.IsNotNull(result1);
        Assert.IsNotNull(result2);
    }

    [TestMethod]
    public async Task GetProductByIdAsync_ShouldReturnCorrectProduct()
    {
        // Arrange
        var testProduct = _testProducts[0];
        _mockRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(testProduct);

        // Act
        var result = await _service.GetProductByIdAsync(1);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(testProduct.Id, result.Id);
        Assert.AreEqual(testProduct.Name, result.Name);
    }
}