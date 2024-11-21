using ProductManagement.Core.Delegates;
using ProductManagement.Core.Entities;

namespace ProductManagement.Core.Interfaces;


public interface IProductService
{
    event ProductCreatedDelegate OnProductCreated;
    event ProductUpdatedDelegate OnProductUpdated;
    event ProductDeletedDelegate OnProductDeleted;
    event ProductPriceLowDelegate OnProductPriceLow;

    Task<IEnumerable<Product>> GetAllProductsAsync();
    Task<Product?> GetProductByIdAsync(int id);
    Task<Product> CreateProductAsync(Product product);
    Task UpdateProductAsync(Product product);
    Task DeleteProductAsync(int id);
    Task<Dictionary<string, decimal>> GetCategoryAnalyticsAsync();
    Task<string> GetHighestStockValueCategoryAsync();
    void SetPriceThreshold(decimal threshold);
}