using Microsoft.AspNetCore.Mvc;
using ProductManagement.Core.Entities;
using ProductManagement.Core.Interfaces;
using ProductManagement.Core.Services;

namespace ProductManagement.Web.Controllers;

public class ProductController : Controller
{
    private readonly IProductService _productService;
    private readonly INotificationService _notificationService;


    public ProductController(IProductService productService, INotificationService notificationService)
    {
        _productService = productService;
        _notificationService = notificationService;


        _productService.OnProductCreated += _notificationService.SendProductCreatedNotification;
        _productService.OnProductUpdated += _notificationService.SendProductUpdatedNotification;
        _productService.OnProductDeleted += _notificationService.SendProductDeletedNotification;
        _productService.OnProductPriceLow += _notificationService.SendPriceLowNotification;

   
        _productService.SetPriceThreshold(100.0m);
    }

    public async Task<IActionResult> Index()
    {
        var products = await _productService.GetAllProductsAsync();
        return View(products);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Product product)
    {
        if (ModelState.IsValid)
        {
            await _productService.CreateProductAsync(product);
            TempData["Success"] = "Product created successfully!";
            return RedirectToAction(nameof(Index));
        }
        return View(product);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }
        return View(product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Product product)
    {
        if (id != product.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            await _productService.UpdateProductAsync(product);
            TempData["Success"] = "Product updated successfully!";
            return RedirectToAction(nameof(Index));
        }
        return View(product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _productService.DeleteProductAsync(id);
        TempData["Success"] = "Product deleted successfully!";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Report()
    {
        var analytics = await _productService.GetCategoryAnalyticsAsync();
        var highestStockCategory = await _productService.GetHighestStockValueCategoryAsync();

        ViewBag.HighestStockCategory = highestStockCategory;
        return View(analytics);
    }
}