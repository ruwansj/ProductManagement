using Microsoft.Extensions.Caching.Memory;
using ProductManagement.Core.Delegates;
using ProductManagement.Core.Entities;
using ProductManagement.Core.Interfaces;

namespace ProductManagement.Core.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;
    private readonly IMemoryCache _cache;
    private readonly string AllProductsCacheKey = "AllProducts";
    private readonly string CategoryAnalyticsCacheKey = "CategoryAnalytics";
    private decimal _priceThreshold = 100.0m; 

    public event ProductCreatedDelegate? OnProductCreated;
    public event ProductUpdatedDelegate? OnProductUpdated;
    public event ProductDeletedDelegate? OnProductDeleted;
    public event ProductPriceLowDelegate? OnProductPriceLow;

    public ProductService(IProductRepository repository, IMemoryCache cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public void SetPriceThreshold(decimal threshold)
    {
        _priceThreshold = threshold;
    }

    public async Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        return await _cache.GetOrCreateAsync(AllProductsCacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
            return await _repository.GetAllAsync();
        });
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<Product> CreateProductAsync(Product product)
    {
        var result = await _repository.CreateAsync(product);
        _cache.Remove(AllProductsCacheKey);
        _cache.Remove(CategoryAnalyticsCacheKey);

        OnProductCreated?.Invoke(result);

        // Check price threshold
        if (product.Price < _priceThreshold)
        {
            OnProductPriceLow?.Invoke(product, _priceThreshold);
        }

        return result;
    }

    public async Task UpdateProductAsync(Product product)
    {
        await _repository.UpdateAsync(product);
        _cache.Remove(AllProductsCacheKey);
        _cache.Remove(CategoryAnalyticsCacheKey);

        OnProductUpdated?.Invoke(product);

        if (product.Price < _priceThreshold)
        {
            OnProductPriceLow?.Invoke(product, _priceThreshold);
        }
    }

    public async Task DeleteProductAsync(int id)
    {
        await _repository.DeleteAsync(id);
        _cache.Remove(AllProductsCacheKey);
        _cache.Remove(CategoryAnalyticsCacheKey);

        OnProductDeleted?.Invoke(id);
    }

    public async Task<Dictionary<string, decimal>> GetCategoryAnalyticsAsync()
    {
        return await _cache.GetOrCreateAsync(CategoryAnalyticsCacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
            return await _repository.GetCategoryAveragesAsync();
        });
    }

    public async Task<string> GetHighestStockValueCategoryAsync()
    {
        return await _repository.GetHighestStockValueCategoryAsync();
    }
}