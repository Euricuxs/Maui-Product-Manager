using MauiProductManager.Tests.Fakes;
using MauiProductManager.ViewModels;

namespace MauiProductManager.Tests.ViewModels;

public class CreateProductViewModelTests
{
    private readonly FakeProductService _fakeService = new();
    private readonly CreateProductViewModel _vm;

    public CreateProductViewModelTests()
    {
        _vm = new CreateProductViewModel(_fakeService);
    }

    private async Task ExecuteSave()
    {
        await _vm.SaveCommand.ExecuteAsync(null);
    }

    [Fact]
    public async Task Save_WhenNameIsEmpty_DoesNotCallCreate()
    {
        _vm.Name = "";
        _vm.Category = "Electronics";
        _vm.Price = "100";

        await ExecuteSave();

        Assert.Equal(0, _fakeService.CreateCallCount);
    }

    [Fact]
    public async Task Save_WhenNameIsWhitespace_DoesNotCallCreate()
    {
        _vm.Name = "   ";
        _vm.Category = "Electronics";
        _vm.Price = "100";

        await ExecuteSave();

        Assert.Equal(0, _fakeService.CreateCallCount);
    }

    [Fact]
    public async Task Save_WhenNameIsTooLong_DoesNotCallCreate()
    {
        _vm.Name = new string('a', 101);
        _vm.Category = "Electronics";
        _vm.Price = "100";

        await ExecuteSave();

        Assert.Equal(0, _fakeService.CreateCallCount);
        Assert.NotNull(_vm.NameError);
    }

    [Fact]
    public async Task Save_WhenCategoryIsEmpty_DoesNotCallCreate()
    {
        _vm.Name = "Laptop";
        _vm.Category = "";
        _vm.Price = "100";

        await ExecuteSave();

        Assert.Equal(0, _fakeService.CreateCallCount);
    }

    [Fact]
    public async Task Save_WhenCategoryIsWhitespace_DoesNotCallCreate()
    {
        _vm.Name = "Laptop";
        _vm.Category = "   ";
        _vm.Price = "100";

        await ExecuteSave();

        Assert.Equal(0, _fakeService.CreateCallCount);
    }

    [Fact]
    public async Task Save_WhenPriceIsZero_DoesNotCallCreate()
    {
        _vm.Name = "Laptop";
        _vm.Category = "Electronics";
        _vm.Price = "0";

        await ExecuteSave();

        Assert.Equal(0, _fakeService.CreateCallCount);
    }

    [Fact]
    public async Task Save_WhenPriceIsNegative_DoesNotCallCreate()
    {
        _vm.Name = "Laptop";
        _vm.Category = "Electronics";
        _vm.Price = "-10";

        await ExecuteSave();

        Assert.Equal(0, _fakeService.CreateCallCount);
    }

    [Fact]
    public async Task Save_WhenPriceIsInvalid_DoesNotCallCreate()
    {
        _vm.Name = "Laptop";
        _vm.Category = "Electronics";
        _vm.Price = "abc";

        await ExecuteSave();

        Assert.Equal(0, _fakeService.CreateCallCount);
    }

    [Fact]
    public async Task Save_WhenPriceExceedsMax_DoesNotCallCreate()
    {
        _vm.Name = "Laptop";
        _vm.Category = "Electronics";
        _vm.Price = "1000001";

        await ExecuteSave();

        Assert.Equal(0, _fakeService.CreateCallCount);
    }

    [Fact]
    public async Task Save_WhenValid_CallsCreateProductAsync()
    {
        _vm.Name = "Laptop";
        _vm.Category = "Electronics";
        _vm.Price = "999.99";
        _fakeService.ExceptionToThrow = new Exception("fail after save");

        await ExecuteSave();

        Assert.Equal(1, _fakeService.CreateCallCount);
    }

    [Fact]
    public async Task Save_WhenValid_CallsCreateWithCorrectValues()
    {
        _vm.Name = "  Laptop  ";
        _vm.Category = "  Electronics  ";
        _vm.Price = "1000";
        _fakeService.ExceptionToThrow = new Exception("stop before navigation");

        await ExecuteSave();

        Assert.NotNull(_fakeService.LastCreatedProduct);
        Assert.Equal("Laptop", _fakeService.LastCreatedProduct.Name);
        Assert.Equal(1000m, _fakeService.LastCreatedProduct.Price);
        Assert.Equal("Electronics", _fakeService.LastCreatedProduct.Category);
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

        Assert.Equal(0, _fakeService.CreateCallCount);
    }

    [Fact]
    public async Task Save_WhenNameTooLong_SetsNameError()
    {
        _vm.Name = new string('a', 101);
        _vm.Category = "Electronics";
        _vm.Price = "100";

        await ExecuteSave();

        Assert.NotNull(_vm.NameError);
    }

    [Fact]
    public async Task Save_WhenCategoryEmpty_SetsCategoryError()
    {
        _vm.Name = "Laptop";
        _vm.Category = "";
        _vm.Price = "100";

        await ExecuteSave();

        Assert.NotNull(_vm.CategoryError);
    }

    [Fact]
    public async Task Save_WhenPriceInvalid_SetsPriceError()
    {
        _vm.Name = "Laptop";
        _vm.Category = "Electronics";
        _vm.Price = "abc";

        await ExecuteSave();

        Assert.NotNull(_vm.PriceError);
    }

    [Fact]
    public async Task Save_WhenCorrectingInput_ClearsError()
    {
        _vm.Name = "";
        _vm.Category = "Electronics";
        _vm.Price = "100";
        await ExecuteSave();
        Assert.NotNull(_vm.NameError);

        _vm.Name = "Laptop";
        await ExecuteSave();
        Assert.Null(_vm.NameError);
    }
}
