namespace MauiProductManager.Api.Models;

/// <summary>
/// Represents a product in the catalog.
/// </summary>
public class Product
{
    /// <summary>
    /// Unique identifier for the product.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Name of the product. Required. Maximum 100 characters.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Price of the product. Must be greater than zero and at most 1,000,000.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Category the product belongs to. Required. Maximum 50 characters.
    /// </summary>
    public string Category { get; set; } = string.Empty;
}
