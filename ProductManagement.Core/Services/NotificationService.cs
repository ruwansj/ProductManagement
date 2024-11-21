
using global::ProductManagement.Core.Entities;

using Microsoft.Extensions.Logging;
using ProductManagement.Core.Services;
public class NotificationService : INotificationService
{
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(ILogger<NotificationService> logger)
    {
        _logger = logger;
    }

    public void SendProductCreatedNotification(Product product)
    {
        _logger.LogInformation("Product created: {Name} with ID {Id}", product.Name, product.Id);
        // Here you could send emails, push notifications, etc.
    }

    public void SendProductUpdatedNotification(Product product)
    {
        _logger.LogInformation("Product updated: {Name} with ID {Id}", product.Name, product.Id);
    }

    public void SendProductDeletedNotification(int productId)
    {
        _logger.LogInformation("Product deleted with ID {Id}", productId);
    }

    public void SendPriceLowNotification(Product product, decimal threshold)
    {
        _logger.LogWarning(
            "Product {Name} price ({Price}) is below threshold ({Threshold})",
            product.Name,
            product.Price,
            threshold
        );
    }
}

namespace ProductManagement.Core.Services
{

    public interface INotificationService
    {
        void SendProductCreatedNotification(Product product);
        void SendProductUpdatedNotification(Product product);
        void SendProductDeletedNotification(int productId);
        void SendPriceLowNotification(Product product, decimal threshold);
    }

    public class NotificationService : INotificationService
    {
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(ILogger<NotificationService> logger)
        {
            _logger = logger;
        }

        public void SendProductCreatedNotification(Product product)
        {
            _logger.LogInformation("Product created: {Name} with ID {Id}", product.Name, product.Id);
            // Here you could send emails, push notifications, etc.
        }

        public void SendProductUpdatedNotification(Product product)
        {
            _logger.LogInformation("Product updated: {Name} with ID {Id}", product.Name, product.Id);
        }

        public void SendProductDeletedNotification(int productId)
        {
            _logger.LogInformation("Product deleted with ID {Id}", productId);
        }

        public void SendPriceLowNotification(Product product, decimal threshold)
        {
            _logger.LogWarning(
                "Product {Name} price ({Price}) is below threshold ({Threshold})",
                product.Name,
                product.Price,
                threshold
            );
        }
    }
}
