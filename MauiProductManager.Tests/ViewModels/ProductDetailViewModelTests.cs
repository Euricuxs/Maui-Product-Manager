using System.Net;
using MauiProductManager.Models;
using MauiProductManager.Tests.Fakes;
using MauiProductManager.ViewModels;

namespace MauiProductManager.Tests.ViewModels;

public class ProductDetailViewModelTests
{
    private readonly FakeProductService _fakeService = new();
    private bool _confirmResult = true;
    private readonly ProductDetailViewModel _vm;

    public ProductDetailViewModelTests()
    {
        _vm = new ProductDetailViewModel(
            _fakeService,
            (title, message, accept, cancel) => Task.FromResult(_confirmResult));
    }

    private static Product MakeProduct(int id, string name, decimal price, string category) =>
        new() { Id = id, Name = name, Price = price, Category = category };

    private async Task ExecuteDelete()
    {
        await _vm.DeleteCommand.ExecuteAsync(null);
    }

    [Fact]
    public async Task LoadProductAsync_WhenProductExists_PopulatesProduct()
    {
        _fakeService.SingleProductToReturn = MakeProduct(1, "Laptop", 999.99m, "Electronics");

        await _vm.LoadProductAsync(1);

        Assert.NotNull(_vm.Product);
        Assert.Equal("Laptop", _vm.Product.Name);
        Assert.True(_vm.HasProduct);
    }

    [Fact]
    public async Task LoadProductAsync_WhenProductNotFound_SetsErrorState()
    {
        var ex = new HttpRequestException("not found") { Data = { ["StatusCode"] = 404 } };
        _fakeService.ExceptionToThrow = ex;

        await _vm.LoadProductAsync(999);

        Assert.True(_vm.HasError);
        Assert.Contains("could not", _vm.ErrorMessage, StringComparison.OrdinalIgnoreCase);
        Assert.False(_vm.HasProduct);
    }

    [Fact]
    public async Task LoadProductAsync_WhenNetworkError_SetsErrorState()
    {
        _fakeService.ExceptionToThrow = new HttpRequestException("network");

        await _vm.LoadProductAsync(1);

        Assert.True(_vm.HasError);
        Assert.NotEmpty(_vm.ErrorMessage);
    }

    [Fact]
    public async Task LoadProductAsync_WhenCancelled_DoesNotSetError()
    {
        _fakeService.ExceptionToThrow = new OperationCanceledException();
        var cts = new CancellationTokenSource();
        cts.Cancel();

        await _vm.LoadProductAsync(1, cts.Token);

        Assert.False(_vm.HasError);
    }

    [Fact]
    public async Task Delete_WhenConfirmed_CallsDeleteService()
    {
        _fakeService.SingleProductToReturn = MakeProduct(5, "Laptop", 999.99m, "Electronics");
        await _vm.LoadProductAsync(5);
        _confirmResult = true;
        _fakeService.ExceptionToThrow = new Exception("stop before navigation");

        await ExecuteDelete();

        Assert.Equal(1, _fakeService.DeleteCallCount);
        Assert.Equal(5, _fakeService.LastDeletedId);
    }

    [Fact]
    public async Task Delete_WhenCancelled_DoesNotCallDeleteService()
    {
        _fakeService.SingleProductToReturn = MakeProduct(5, "Laptop", 999.99m, "Electronics");
        await _vm.LoadProductAsync(5);
        _confirmResult = false;

        await ExecuteDelete();

        Assert.Equal(0, _fakeService.DeleteCallCount);
    }

    [Fact]
    public async Task Delete_WhenAlreadyDeleting_DoesNothing()
    {
        _fakeService.SingleProductToReturn = MakeProduct(1, "Test", 10m, "Cat");
        await _vm.LoadProductAsync(1);
        _vm.IsDeleting = true;

        await ExecuteDelete();

        Assert.Equal(0, _fakeService.DeleteCallCount);
    }

    [Fact]
    public async Task Delete_WhenServiceReturns404_SetsErrorState()
    {
        _fakeService.SingleProductToReturn = MakeProduct(5, "Laptop", 999.99m, "Electronics");
        await _vm.LoadProductAsync(5);
        _confirmResult = true;
        var ex = new HttpRequestException("not found") { Data = { ["StatusCode"] = 404 } };
        _fakeService.ExceptionToThrow = ex;

        await ExecuteDelete();

        Assert.True(_vm.HasError);
        Assert.Contains("could not", _vm.ErrorMessage, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Delete_WhenServiceFails_SetsErrorState()
    {
        _fakeService.SingleProductToReturn = MakeProduct(5, "Laptop", 999.99m, "Electronics");
        await _vm.LoadProductAsync(5);
        _confirmResult = true;
        _fakeService.ExceptionToThrow = new HttpRequestException("network error");

        await ExecuteDelete();

        Assert.True(_vm.HasError);
        Assert.NotEmpty(_vm.ErrorMessage);
    }

    [Fact]
    public async Task Delete_WhenProductIsNull_DoesNothing()
    {
        _vm.Product = null;

        await ExecuteDelete();

        Assert.Equal(0, _fakeService.DeleteCallCount);
    }
}
