using System.ComponentModel.DataAnnotations;

namespace ProductManagement.Core.Entities;

public class Product
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    [Display(Name = "Product Name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Category { get; set; } = string.Empty;

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    [DataType(DataType.Currency)]
    public decimal Price { get; set; }

    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "Stock must be greater than or equal to 0")]
    public int Stock { get; set; }
}
