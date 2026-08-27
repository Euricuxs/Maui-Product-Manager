using System.Collections.ObjectModel;
using MauiProductManager.Models;

namespace MauiProductManager.Services;

public interface IProductService
{
    // Synchronous methods (used by existing in-memory implementation)
    ObservableCollection<Product> GetAll();
    void Add(Product product);
    void Update(Product product);
    void Delete(int id);
    Product? GetById(int id);

    // Asynchronous methods (used by API implementation)
    Task<ObservableCollection<Product>> GetProductsAsync(CancellationToken cancellationToken = default);
    Task<Product?> GetProductAsync(int id, CancellationToken cancellationToken = default);
    Task<Product> CreateProductAsync(Product product);
    Task UpdateProductAsync(Product product);
    Task DeleteProductAsync(int id);
}
