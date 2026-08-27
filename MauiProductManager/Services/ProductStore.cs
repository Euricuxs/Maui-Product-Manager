using MauiProductManager.Models;

namespace MauiProductManager.Services;

public static class ProductStore
{
    public static List<Product> Products { get; } = new()
    {
        new Product { Id = 1, Name = "Laptop", Price = 999.99m, Category = "Electronics" },
        new Product { Id = 2, Name = "Coffee Mug", Price = 12.50m, Category = "Kitchen" },
        new Product { Id = 3, Name = "Desk Chair", Price = 249.00m, Category = "Furniture" },
    };

    public static Product? GetById(int id) => Products.FirstOrDefault(p => p.Id == id);
}
