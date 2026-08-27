using System.Net;
using System.Net.Http.Json;
using MauiProductManager.Models;
using MauiProductManager.Services;

namespace MauiProductManager.Tests.Services;

public class ProductServiceTests
{
    private const string BaseUrl = "http://test.example.com";

    private static ProductService CreateService(HttpMessageHandler handler) =>
        new(new HttpClient(handler) { BaseAddress = new Uri(BaseUrl) });

    private static Product Product(int id, string name, decimal price, string category) =>
        new() { Id = id, Name = name, Price = price, Category = category };

    private class TestHttpMessageHandler : HttpMessageHandler
    {
        public Func<HttpRequestMessage, CancellationToken, HttpResponseMessage>? SendAsyncFunc { get; set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
            Task.FromResult(SendAsyncFunc!(request, cancellationToken));
    }

    [Fact]
    public async Task GetProductsAsync_ReturnsProducts()
    {
        var products = new[] { Product(1, "Laptop", 999.99m, "Electronics") };
        HttpRequestMessage? capturedRequest = null;
        var handler = new TestHttpMessageHandler
        {
            SendAsyncFunc = (req, _) =>
            {
                capturedRequest = req;
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = JsonContent.Create(products)
                };
            }
        };

        var service = CreateService(handler);
        var result = await service.GetProductsAsync();

        Assert.NotNull(capturedRequest);
        Assert.Equal(HttpMethod.Get, capturedRequest.Method);
        Assert.Equal("/api/products", capturedRequest.RequestUri!.PathAndQuery);
        Assert.Single(result);
        Assert.Equal("Laptop", result[0].Name);
    }

    [Fact]
    public async Task GetProductsAsync_WhenEmpty_ReturnsEmptyCollection()
    {
        var handler = new TestHttpMessageHandler
        {
            SendAsyncFunc = (_, _) => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(Array.Empty<Product>())
            }
        };

        var service = CreateService(handler);
        var result = await service.GetProductsAsync();

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetProductAsync_WhenExists_ReturnsProduct()
    {
        var product = Product(5, "Laptop", 999.99m, "Electronics");
        HttpRequestMessage? capturedRequest = null;
        var handler = new TestHttpMessageHandler
        {
            SendAsyncFunc = (req, _) =>
            {
                capturedRequest = req;
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = JsonContent.Create(product)
                };
            }
        };

        var service = CreateService(handler);
        var result = await service.GetProductAsync(5);

        Assert.NotNull(capturedRequest);
        Assert.Equal(HttpMethod.Get, capturedRequest.Method);
        Assert.Equal("/api/products/5", capturedRequest.RequestUri!.PathAndQuery);
        Assert.NotNull(result);
        Assert.Equal("Laptop", result.Name);
    }

    [Fact]
    public async Task GetProductAsync_When404_ThrowsHttpRequestException()
    {
        var handler = new TestHttpMessageHandler
        {
            SendAsyncFunc = (_, _) => new HttpResponseMessage(HttpStatusCode.NotFound)
        };

        var service = CreateService(handler);

        var ex = await Assert.ThrowsAsync<HttpRequestException>(() => service.GetProductAsync(999));

        Assert.True(ex.Data.Contains("StatusCode"));
        Assert.Equal(404, (int)ex.Data["StatusCode"]!);
    }

    [Fact]
    public async Task GetProductAsync_WhenServerError_ThrowsHttpRequestException()
    {
        var handler = new TestHttpMessageHandler
        {
            SendAsyncFunc = (_, _) => new HttpResponseMessage(HttpStatusCode.InternalServerError)
        };

        var service = CreateService(handler);

        var ex = await Assert.ThrowsAsync<HttpRequestException>(() => service.GetProductAsync(1));

        Assert.True(ex.Data.Contains("StatusCode"));
        Assert.Equal(500, (int)ex.Data["StatusCode"]!);
    }

    [Fact]
    public async Task CreateProductAsync_WhenValid_ReturnsProduct()
    {
        var product = Product(1, "Laptop", 999.99m, "Electronics");
        HttpRequestMessage? capturedRequest = null;
        var handler = new TestHttpMessageHandler
        {
            SendAsyncFunc = (req, _) =>
            {
                capturedRequest = req;
                return new HttpResponseMessage(HttpStatusCode.Created)
                {
                    Content = JsonContent.Create(product)
                };
            }
        };

        var service = CreateService(handler);
        var result = await service.CreateProductAsync(product);

        Assert.NotNull(capturedRequest);
        Assert.Equal(HttpMethod.Post, capturedRequest.Method);
        Assert.Equal("/api/products", capturedRequest.RequestUri!.PathAndQuery);
        Assert.Equal("Laptop", result.Name);
    }

    [Fact]
    public async Task CreateProductAsync_WhenServerError_Throws()
    {
        var product = Product(1, "Laptop", 999.99m, "Electronics");
        var handler = new TestHttpMessageHandler
        {
            SendAsyncFunc = (_, _) => new HttpResponseMessage(HttpStatusCode.InternalServerError)
        };

        var service = CreateService(handler);

        await Assert.ThrowsAsync<HttpRequestException>(() => service.CreateProductAsync(product));
    }

    [Fact]
    public async Task UpdateProductAsync_WhenValid_CallsApi()
    {
        var product = Product(5, "Laptop", 999.99m, "Electronics");
        HttpRequestMessage? capturedRequest = null;
        var handler = new TestHttpMessageHandler
        {
            SendAsyncFunc = (req, _) =>
            {
                capturedRequest = req;
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = JsonContent.Create(product)
                };
            }
        };

        var service = CreateService(handler);
        await service.UpdateProductAsync(product);

        Assert.NotNull(capturedRequest);
        Assert.Equal(HttpMethod.Put, capturedRequest.Method);
        Assert.Equal("/api/products/5", capturedRequest.RequestUri!.PathAndQuery);
    }

    [Fact]
    public async Task UpdateProductAsync_When404_ThrowsHttpRequestException()
    {
        var product = Product(999, "Laptop", 999.99m, "Electronics");
        var handler = new TestHttpMessageHandler
        {
            SendAsyncFunc = (_, _) => new HttpResponseMessage(HttpStatusCode.NotFound)
        };

        var service = CreateService(handler);

        var ex = await Assert.ThrowsAsync<HttpRequestException>(() => service.UpdateProductAsync(product));

        Assert.Equal(404, (int)ex.Data["StatusCode"]!);
    }

    [Fact]
    public async Task DeleteProductAsync_WhenValid_CallsApi()
    {
        HttpRequestMessage? capturedRequest = null;
        var handler = new TestHttpMessageHandler
        {
            SendAsyncFunc = (req, _) =>
            {
                capturedRequest = req;
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
        };

        var service = CreateService(handler);
        await service.DeleteProductAsync(5);

        Assert.NotNull(capturedRequest);
        Assert.Equal(HttpMethod.Delete, capturedRequest.Method);
        Assert.Equal("/api/products/5", capturedRequest.RequestUri!.PathAndQuery);
    }

    [Fact]
    public async Task DeleteProductAsync_When404_ThrowsHttpRequestException()
    {
        var handler = new TestHttpMessageHandler
        {
            SendAsyncFunc = (_, _) => new HttpResponseMessage(HttpStatusCode.NotFound)
        };

        var service = CreateService(handler);

        var ex = await Assert.ThrowsAsync<HttpRequestException>(() => service.DeleteProductAsync(999));

        Assert.Equal(404, (int)ex.Data["StatusCode"]!);
    }
}
