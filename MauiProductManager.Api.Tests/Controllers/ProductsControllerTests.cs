using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MauiProductManager.Api.Controllers;
using MauiProductManager.Api.Data;
using MauiProductManager.Api.Models;

namespace MauiProductManager.Api.Tests.Controllers;

public class ProductsControllerTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly ProductsController _controller;

    public ProductsControllerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);
        _controller = new ProductsController(_context);
    }

    public void Dispose() => _context.Dispose();

    private static ProductDto ValidDto(string name = "Laptop", decimal price = 999.99m, string category = "Electronics") =>
        new() { Name = name, Price = price, Category = category };

    private async Task<Product> CreateProductInDb(string name, decimal price, string category)
    {
        var product = new Product { Name = name, Price = price, Category = category };
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return product;
    }

    private void ValidateDto(ProductDto dto)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(dto);
        Validator.TryValidateObject(dto, validationContext, validationResults, true);
        foreach (var error in validationResults)
        {
            _controller.ModelState.AddModelError(error.MemberNames.First(), error.ErrorMessage);
        }
    }

    [Fact]
    public async Task GetProducts_ReturnsOkWithProducts()
    {
        await CreateProductInDb("Laptop", 999.99m, "Electronics");
        await CreateProductInDb("Mouse", 29.99m, "Electronics");

        var result = await _controller.GetProducts();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var products = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);
        Assert.Equal(2, products.Count());
    }

    [Fact]
    public async Task GetProducts_WhenEmpty_ReturnsEmptyList()
    {
        var result = await _controller.GetProducts();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var products = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);
        Assert.Empty(products);
    }

    [Fact]
    public async Task GetProduct_WhenExists_ReturnsOk()
    {
        var product = await CreateProductInDb("Laptop", 999.99m, "Electronics");

        var result = await _controller.GetProduct(product.Id);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsType<Product>(okResult.Value);
        Assert.Equal("Laptop", returned.Name);
    }

    [Fact]
    public async Task GetProduct_WhenNotExists_Returns404()
    {
        var result = await _controller.GetProduct(999);

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task CreateProduct_WhenValid_ReturnsCreated()
    {
        var dto = ValidDto("New Laptop", 1500m, "Tech");

        var result = await _controller.CreateProduct(dto);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(201, createdResult.StatusCode);
        var product = Assert.IsType<Product>(createdResult.Value);
        Assert.Equal("New Laptop", product.Name);
        Assert.Equal(1500m, product.Price);
    }

    [Fact]
    public async Task CreateProduct_WhenValid_PersistsProduct()
    {
        var dto = ValidDto("New Laptop", 1500m, "Tech");

        await _controller.CreateProduct(dto);

        var count = await _context.Products.CountAsync();
        Assert.Equal(1, count);
    }

    [Fact]
    public async Task CreateProduct_WhenNameEmpty_Returns400()
    {
        var dto = ValidDto("", 100m, "Electronics");
        ValidateDto(dto);

        var result = await _controller.CreateProduct(dto);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task CreateProduct_WhenNameTooLong_Returns400()
    {
        var dto = ValidDto(new string('a', 101), 100m, "Electronics");
        ValidateDto(dto);

        var result = await _controller.CreateProduct(dto);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task CreateProduct_WhenPriceZero_Returns400()
    {
        var dto = ValidDto("Laptop", 0m, "Electronics");
        ValidateDto(dto);

        var result = await _controller.CreateProduct(dto);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task CreateProduct_WhenPriceNegative_Returns400()
    {
        var dto = ValidDto("Laptop", -10m, "Electronics");
        ValidateDto(dto);

        var result = await _controller.CreateProduct(dto);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task CreateProduct_WhenPriceTooHigh_Returns400()
    {
        var dto = ValidDto("Laptop", 1000001m, "Electronics");
        ValidateDto(dto);

        var result = await _controller.CreateProduct(dto);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task CreateProduct_WhenCategoryEmpty_Returns400()
    {
        var dto = ValidDto("Laptop", 100m, "");
        ValidateDto(dto);

        var result = await _controller.CreateProduct(dto);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task CreateProduct_WhenCategoryTooLong_Returns400()
    {
        var dto = ValidDto("Laptop", 100m, new string('a', 51));
        ValidateDto(dto);

        var result = await _controller.CreateProduct(dto);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task UpdateProduct_WhenExists_ReturnsOk()
    {
        var product = await CreateProductInDb("Old Laptop", 500m, "Old Tech");
        var dto = ValidDto("Updated Laptop", 1500m, "New Tech");

        var result = await _controller.UpdateProduct(product.Id, dto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var updated = Assert.IsType<Product>(okResult.Value);
        Assert.Equal("Updated Laptop", updated.Name);
        Assert.Equal(1500m, updated.Price);
        Assert.Equal("New Tech", updated.Category);
    }

    [Fact]
    public async Task UpdateProduct_WhenNotExists_Returns404()
    {
        var dto = ValidDto("Updated", 100m, "Tech");

        var result = await _controller.UpdateProduct(999, dto);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task UpdateProduct_WhenInvalidData_Returns400()
    {
        var product = await CreateProductInDb("Laptop", 500m, "Tech");
        var dto = ValidDto("", 0m, "");
        ValidateDto(dto);

        var result = await _controller.UpdateProduct(product.Id, dto);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task DeleteProduct_WhenExists_ReturnsOk()
    {
        var product = await CreateProductInDb("Laptop", 999.99m, "Electronics");

        var result = await _controller.DeleteProduct(product.Id);

        Assert.IsType<OkObjectResult>(result);
        var count = await _context.Products.CountAsync();
        Assert.Equal(0, count);
    }

    [Fact]
    public async Task DeleteProduct_WhenNotExists_Returns404()
    {
        var result = await _controller.DeleteProduct(999);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task DeleteProduct_WhenExists_RemovesFromDatabase()
    {
        var product = await CreateProductInDb("Laptop", 999.99m, "Electronics");
        var id = product.Id;

        await _controller.DeleteProduct(id);

        var exists = await _context.Products.AnyAsync(p => p.Id == id);
        Assert.False(exists);
    }
}
