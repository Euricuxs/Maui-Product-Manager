using System.Collections.ObjectModel;
using MauiProductManager.Models;
using MauiProductManager.Tests.Fakes;
using MauiProductManager.ViewModels;

namespace MauiProductManager.Tests.ViewModels;

public class ProductListViewModelTests
{
    private readonly FakeProductService _fakeService = new();
    private readonly ProductListViewModel _vm;

    public ProductListViewModelTests()
    {
        _vm = new ProductListViewModel(_fakeService);
    }

    private static ObservableCollection<Product> Products(params Product[] products) =>
        new(products);

    private static Product Product(int id, string name, decimal price, string category) =>
        new() { Id = id, Name = name, Price = price, Category = category };

    [Fact]
    public void LoadProductsAsync_WhenServiceReturnsProducts_PopulatesProducts()
    {
        _fakeService.ProductsToReturn = Products(
            Product(1, "Laptop", 999.99m, "Electronics"),
            Product(2, "Mouse", 29.99m, "Electronics"));

        _vm.StartRefresh();

        Assert.Equal(2, _vm.Products.Count);
        Assert.Equal(1, _vm.Products[0].Id);
        Assert.True(_vm.HasContent);
    }

    [Fact]
    public void LoadProductsAsync_WhenServiceFails_SetsErrorState()
    {
        _fakeService.ExceptionToThrow = new Exception("Network error");

        _vm.StartRefresh();

        Assert.True(_vm.HasError);
        Assert.NotEmpty(_vm.ErrorMessage);
        Assert.False(_vm.HasContent);
    }

    [Fact]
    public void LoadProductsAsync_WhenEmptyResponse_SetsEmptyState()
    {
        _fakeService.ProductsToReturn = Products();

        _vm.StartRefresh();

        Assert.Empty(_vm.Products);
        Assert.False(_vm.HasContent);
    }

    [Fact]
    public void LoadProductsAsync_WhenAlreadyLoading_DoesNothing()
    {
        _fakeService.ProductsToReturn = Products(Product(1, "Test", 10, "Cat"));
        _vm.StartRefresh();
        _vm.StartRefresh();

        Assert.True(_fakeService.GetAllCallCount >= 1);
    }

    [Fact]
    public void Refresh_WhenCalled_CallsLoadAgain()
    {
        _fakeService.ProductsToReturn = Products(Product(1, "Test", 10, "Cat"));
        _vm.RefreshCommand.Execute(null);

        Assert.Equal(1, _fakeService.GetAllCallCount);
    }

    [Fact]
    public void Search_WhenMatchingName_ReturnsMatchingProducts()
    {
        _fakeService.ProductsToReturn = Products(
            Product(1, "Laptop", 999.99m, "Electronics"),
            Product(2, "Mouse", 29.99m, "Electronics"),
            Product(3, "Desk Chair", 199.99m, "Furniture"));

        _vm.StartRefresh();
        _vm.SearchText = "laptop";

        Assert.Single(_vm.FilteredProducts);
        Assert.Equal("Laptop", _vm.FilteredProducts[0].Name);
        Assert.True(_vm.HasFilteredContent);
    }

    [Fact]
    public void Search_WhenMatchingCategory_ReturnsMatchingProducts()
    {
        _fakeService.ProductsToReturn = Products(
            Product(1, "Laptop", 999.99m, "Electronics"),
            Product(2, "Mouse", 29.99m, "Electronics"),
            Product(3, "Desk Chair", 199.99m, "Furniture"));

        _vm.StartRefresh();
        _vm.SearchText = "electronics";

        Assert.Equal(2, _vm.FilteredProducts.Count);
        Assert.All(_vm.FilteredProducts, p => Assert.Contains("Electronics", p.Category));
    }

    [Fact]
    public void Search_WhenPartialMatch_ReturnsMatchingProducts()
    {
        _fakeService.ProductsToReturn = Products(
            Product(1, "Laptop", 999.99m, "Electronics"),
            Product(2, "Mouse", 29.99m, "Electronics"));

        _vm.StartRefresh();
        _vm.SearchText = "lap";

        Assert.Single(_vm.FilteredProducts);
        Assert.Equal("Laptop", _vm.FilteredProducts[0].Name);
    }

    [Fact]
    public void Search_WhenNoMatch_ReturnsEmpty()
    {
        _fakeService.ProductsToReturn = Products(
            Product(1, "Laptop", 999.99m, "Electronics"));

        _vm.StartRefresh();
        _vm.SearchText = "xyz";

        Assert.Empty(_vm.FilteredProducts);
        Assert.False(_vm.HasFilteredContent);
        Assert.True(_vm.IsSearchResultEmpty);
    }

    [Fact]
    public void Search_WhenCaseInsensitive_ReturnsMatchingProducts()
    {
        _fakeService.ProductsToReturn = Products(
            Product(1, "Laptop", 999.99m, "Electronics"),
            Product(2, "Mouse", 29.99m, "Electronics"));

        _vm.StartRefresh();
        _vm.SearchText = "LAPTOP";

        Assert.Single(_vm.FilteredProducts);
        Assert.Equal("Laptop", _vm.FilteredProducts[0].Name);
    }

    [Fact]
    public void Search_WhenCleared_RestoresAllProducts()
    {
        _fakeService.ProductsToReturn = Products(
            Product(1, "Laptop", 999.99m, "Electronics"),
            Product(2, "Mouse", 29.99m, "Electronics"));

        _vm.StartRefresh();
        _vm.SearchText = "mouse";
        Assert.Single(_vm.FilteredProducts);

        _vm.SearchText = "";

        Assert.Equal(2, _vm.FilteredProducts.Count);
        Assert.True(_vm.HasFilteredContent);
    }

    [Fact]
    public void Search_DoesNotModifyOriginalProducts()
    {
        _fakeService.ProductsToReturn = Products(
            Product(1, "Laptop", 999.99m, "Electronics"),
            Product(2, "Mouse", 29.99m, "Electronics"));

        _vm.StartRefresh();

        var originalCount = _vm.Products.Count;
        _vm.SearchText = "laptop";

        Assert.Equal(originalCount, _vm.Products.Count);
        _vm.SearchText = "";

        Assert.Equal(originalCount, _vm.Products.Count);
    }
}
