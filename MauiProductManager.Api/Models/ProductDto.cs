using System.ComponentModel.DataAnnotations;

namespace MauiProductManager.Api.Models;

/// <summary>
/// Data transfer object for creating or updating a product.
/// </summary>
public class ProductDto
{
    /// <summary>
    /// Name of the product. Required. Maximum 100 characters.
    /// </summary>
    /// <example>Mechanical Keyboard</example>
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(100, ErrorMessage = "Name must be 100 characters or fewer.")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Category the product belongs to. Required. Maximum 50 characters.
    /// </summary>
    /// <example>Electronics</example>
    [Required(ErrorMessage = "Category is required.")]
    [StringLength(50, ErrorMessage = "Category must be 50 characters or fewer.")]
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Price of the product. Must be greater than 0 and at most 1,000,000.
    /// </summary>
    /// <example>89.99</example>
    [Range(0.01, 1000000, ErrorMessage = "Price must be greater than zero and 1,000,000 or less.")]
    public decimal Price { get; set; }
}
