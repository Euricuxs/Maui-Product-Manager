using System.Net.Http.Json;
using System.Collections.ObjectModel;
using MauiProductManager.Models;

namespace MauiProductManager.Services;

public class ProductService : IProductService
{
    private readonly HttpClient _httpClient;

    public ProductService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public ObservableCollection<Product> GetAll()
    {
        throw new NotSupportedException("Use GetProductsAsync for the API implementation.");
    }

    public Product? GetById(int id)
    {
        throw new NotSupportedException("Use GetProductAsync for the API implementation.");
    }

    public void Add(Product product)
    {
        throw new NotSupportedException("Use CreateProductAsync for the API implementation.");
    }

    public void Update(Product product)
    {
        throw new NotSupportedException("Use UpdateProductAsync for the API implementation.");
    }

    public void Delete(int id)
    {
        throw new NotSupportedException("Use DeleteProductAsync for the API implementation.");
    }

    public async Task<ObservableCollection<Product>> GetProductsAsync(CancellationToken cancellationToken = default)
    {
        var products = await _httpClient.GetFromJsonAsync<List<Product>>("/api/products", cancellationToken);
        return new ObservableCollection<Product>(products ?? []);
    }

    public async Task<Product?> GetProductAsync(int id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"/api/products/{id}", cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var statusCode = (int)response.StatusCode;
            throw new HttpRequestException($"API returned {statusCode}")
            {
                Data = { ["StatusCode"] = statusCode }
            };
        }
        return await response.Content.ReadFromJsonAsync<Product>(cancellationToken);
    }

    public async Task<Product> CreateProductAsync(Product product)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/products", product);
        if (!response.IsSuccessStatusCode)
        {
            var statusCode = (int)response.StatusCode;
            throw new HttpRequestException($"API returned {statusCode}")
            {
                Data = { ["StatusCode"] = statusCode }
            };
        }
        return (await response.Content.ReadFromJsonAsync<Product>())!;
    }

    public async Task UpdateProductAsync(Product product)
    {
        var response = await _httpClient.PutAsJsonAsync($"/api/products/{product.Id}", product);
        if (!response.IsSuccessStatusCode)
        {
            var statusCode = (int)response.StatusCode;
            throw new HttpRequestException($"API returned {statusCode}")
            {
                Data = { ["StatusCode"] = statusCode }
            };
        }
    }

    public async Task DeleteProductAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"/api/products/{id}");
        if (!response.IsSuccessStatusCode)
        {
            var statusCode = (int)response.StatusCode;
            throw new HttpRequestException($"API returned {statusCode}")
            {
                Data = { ["StatusCode"] = statusCode }
            };
        }
    }
}
