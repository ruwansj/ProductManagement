using ProductManagement.Core.Entities;

namespace ProductManagement.Core.Delegates;

public delegate void ProductCreatedDelegate(Product product);
public delegate void ProductUpdatedDelegate(Product product);
public delegate void ProductDeletedDelegate(int productId);
public delegate void ProductPriceLowDelegate(Product product, decimal threshold);