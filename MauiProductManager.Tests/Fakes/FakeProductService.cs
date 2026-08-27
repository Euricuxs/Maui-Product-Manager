using System.Collections.ObjectModel;
using MauiProductManager.Models;
using MauiProductManager.Services;

namespace MauiProductManager.Tests.Fakes;

public class FakeProductService : IProductService
{
    public ObservableCollection<Product> ProductsToReturn { get; set; } = [];
    public Product? SingleProductToReturn { get; set; }
    public Exception? ExceptionToThrow { get; set; }
    public int GetAllCallCount { get; private set; }
    public int GetProductCallCount { get; private set; }
    public int CreateCallCount { get; private set; }
    public int UpdateCallCount { get; private set; }
    public int DeleteCallCount { get; private set; }
    public Product? LastCreatedProduct { get; private set; }
    public Product? LastUpdatedProduct { get; private set; }
    public int? LastDeletedId { get; private set; }

    public void Reset()
    {
        GetAllCallCount = 0;
        GetProductCallCount = 0;
        CreateCallCount = 0;
        UpdateCallCount = 0;
        DeleteCallCount = 0;
        LastCreatedProduct = null;
        LastUpdatedProduct = null;
        LastDeletedId = null;
        ExceptionToThrow = null;
    }

    public ObservableCollection<Product> GetAll()
    {
        throw new NotSupportedException();
    }

    public Product? GetById(int id)
    {
        throw new NotSupportedException();
    }

    public void Add(Product product)
    {
        throw new NotSupportedException();
    }

    public void Update(Product product)
    {
        throw new NotSupportedException();
    }

    public void Delete(int id)
    {
        throw new NotSupportedException();
    }

    public Task<ObservableCollection<Product>> GetProductsAsync(CancellationToken cancellationToken = default)
    {
        GetAllCallCount++;
        if (ExceptionToThrow != null)
            throw ExceptionToThrow;
        return Task.FromResult(new ObservableCollection<Product>(ProductsToReturn));
    }

    public Task<Product?> GetProductAsync(int id, CancellationToken cancellationToken = default)
    {
        GetProductCallCount++;
        if (ExceptionToThrow != null)
            throw ExceptionToThrow;
        return Task.FromResult(SingleProductToReturn);
    }

    public Task<Product> CreateProductAsync(Product product)
    {
        CreateCallCount++;
        LastCreatedProduct = product;
        if (ExceptionToThrow != null)
            throw ExceptionToThrow;
        return Task.FromResult(product);
    }

    public Task UpdateProductAsync(Product product)
    {
        UpdateCallCount++;
        LastUpdatedProduct = product;
        if (ExceptionToThrow != null)
            throw ExceptionToThrow;
        return Task.CompletedTask;
    }

    public Task DeleteProductAsync(int id)
    {
        DeleteCallCount++;
        LastDeletedId = id;
        if (ExceptionToThrow != null)
            throw ExceptionToThrow;
        return Task.CompletedTask;
    }
}
