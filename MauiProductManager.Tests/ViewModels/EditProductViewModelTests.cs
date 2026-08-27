using System.Net;
using MauiProductManager.Models;
using MauiProductManager.Tests.Fakes;
using MauiProductManager.ViewModels;

namespace MauiProductManager.Tests.ViewModels;

public class EditProductViewModelTests
{
    private readonly FakeProductService _fakeService = new();
    private readonly EditProductViewModel _vm;

    public EditProductViewModelTests()
    {
        _vm = new EditProductViewModel(_fakeService);
    }

    private static Product MakeProduct(int id, string name, decimal price, string category) =>
        new() { Id = id, Name = name, Price = price, Category = category };

    private async Task ExecuteSave()
    {
        await _vm.SaveCommand.ExecuteAsync(null);
    }

    [Fact]
    public async Task LoadProductAsync_WhenProductExists_PopulatesForm()
    {
        _fakeService.SingleProductToReturn = MakeProduct(5, "Laptop", 999.99m, "Electronics");

        await _vm.LoadProductAsync(5);

        Assert.Equal("Laptop", _vm.Name);
        Assert.Equal("999.99", _vm.Price);
        Assert.Equal("Electronics", _vm.Category);
    }

    [Fact]
    public async Task LoadProductAsync_WhenProductNotFound_SetsError()
    {
        _fakeService.SingleProductToReturn = null;

        await _vm.LoadProductAsync(999);

        Assert.True(_vm.HasError);
        Assert.NotEmpty(_vm.ErrorMessage);
    }

    [Fact]
    public async Task LoadProductAsync_WhenNetworkError_SetsError()
    {
        _fakeService.ExceptionToThrow = new HttpRequestException("Network error");

        await _vm.LoadProductAsync(1);

        Assert.True(_vm.HasError);
        Assert.NotEmpty(_vm.ErrorMessage);
    }

    [Fact]
    public async Task LoadProductAsync_WhenCancelled_SetsNoError()
    {
        _fakeService.ExceptionToThrow = new OperationCanceledException();
        var cts = new CancellationTokenSource();
        cts.Cancel();

        await _vm.LoadProductAsync(1, cts.Token);

        Assert.False(_vm.HasError);
    }

    [Fact]
    public async Task Save_WhenNameIsEmpty_DoesNotCallUpdate()
    {
        _vm.Name = "";
        _vm.Category = "Electronics";
        _vm.Price = "100";

        await ExecuteSave();

        Assert.Equal(0, _fakeService.UpdateCallCount);
    }

    [Fact]
    public async Task Save_WhenCategoryIsEmpty_DoesNotCallUpdate()
    {
        _vm.Name = "Laptop";
        _vm.Category = "";
        _vm.Price = "100";

        await ExecuteSave();

        Assert.Equal(0, _fakeService.UpdateCallCount);
    }

    [Fact]
    public async Task Save_WhenPriceIsZero_DoesNotCallUpdate()
    {
        _vm.Name = "Laptop";
        _vm.Category = "Electronics";
        _vm.Price = "0";

        await ExecuteSave();

        Assert.Equal(0, _fakeService.UpdateCallCount);
    }

    [Fact]
    public async Task Save_WhenPriceIsNegative_DoesNotCallUpdate()
    {
        _vm.Name = "Laptop";
        _vm.Category = "Electronics";
        _vm.Price = "-5";

        await ExecuteSave();

        Assert.Equal(0, _fakeService.UpdateCallCount);
    }

    [Fact]
    public async Task Save_WhenValid_CallsUpdateProductAsync()
    {
        _vm.Name = "Laptop";
        _vm.Category = "Electronics";
        _vm.Price = "999.99";
        _fakeService.ExceptionToThrow = new Exception("stop before navigation");

        await ExecuteSave();

        Assert.Equal(1, _fakeService.UpdateCallCount);
    }

    [Fact]
    public async Task Save_WhenValid_UsesExistingProductId()
    {
        _fakeService.SingleProductToReturn = MakeProduct(42, "Old", 100m, "Old");
        await _vm.LoadProductAsync(42);
        _vm.Name = "Updated";
        _vm.Category = "New";
        _vm.Price = "200";
        _fakeService.ExceptionToThrow = new Exception("stop");

        await ExecuteSave();

        Assert.NotNull(_fakeService.LastUpdatedProduct);
        Assert.Equal(42, _fakeService.LastUpdatedProduct.Id);
    }

    [Fact]
    public async Task Save_WhenServiceFails_FormValuesRemain()
    {
        _vm.Name = "Laptop";
        _vm.Category = "Electronics";
        _vm.Price = "999.99";
        _fakeService.ExceptionToThrow = new Exception("API error");

        await ExecuteSave();

        Assert.Equal("Laptop", _vm.Name);
        Assert.Equal("Electronics", _vm.Category);
        Assert.Equal("999.99", _vm.Price);
    }

    [Fact]
    public async Task Save_WhenAlreadyLoading_DoesNothing()
    {
        _vm.Name = "Laptop";
        _vm.Category = "Electronics";
        _vm.Price = "999.99";
        _vm.IsLoading = true;

        await ExecuteSave();

        Assert.Equal(0, _fakeService.UpdateCallCount);
    }

    [Fact]
    public async Task Save_WhenServiceReturns404_SetsErrorMessage()
    {
        _vm.Name = "Laptop";
        _vm.Category = "Electronics";
        _vm.Price = "999.99";
        var ex = new HttpRequestException("not found") { Data = { ["StatusCode"] = 404 } };
        _fakeService.ExceptionToThrow = ex;

        await ExecuteSave();

        Assert.True(_vm.HasError);
        Assert.Contains("removed", _vm.ErrorMessage, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Save_WhenServiceReturnsNetworkError_SetsErrorMessage()
    {
        _vm.Name = "Laptop";
        _vm.Category = "Electronics";
        _vm.Price = "999.99";
        _fakeService.ExceptionToThrow = new HttpRequestException("network error");

        await ExecuteSave();

        Assert.True(_vm.HasError);
        Assert.NotEmpty(_vm.ErrorMessage);
    }

    [Fact]
    public async Task Save_WhenServiceReturns500_SetsErrorMessage()
    {
        _vm.Name = "Laptop";
        _vm.Category = "Electronics";
        _vm.Price = "999.99";
        var ex = new HttpRequestException("server error") { Data = { ["StatusCode"] = 500 } };
        _fakeService.ExceptionToThrow = ex;

        await ExecuteSave();

        Assert.True(_vm.HasError);
    }
}
