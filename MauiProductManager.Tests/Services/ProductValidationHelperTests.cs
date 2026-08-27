using MauiProductManager.Services;

namespace MauiProductManager.Tests.Services;

public class ProductValidationHelperTests
{
    [Fact]
    public void Validate_WhenAllValid_ReturnsNoErrors()
    {
        var (isValid, nameError, priceError, categoryError) =
            ProductValidationHelper.Validate("Laptop", "999.99", "Electronics");

        Assert.True(isValid);
        Assert.Null(nameError);
        Assert.Null(priceError);
        Assert.Null(categoryError);
    }

    [Fact]
    public void Validate_WhenNameEmpty_ReturnsNameError()
    {
        var (_, nameError, _, _) = ProductValidationHelper.Validate("", "100", "Electronics");

        Assert.NotNull(nameError);
    }

    [Fact]
    public void Validate_WhenNameWhitespace_ReturnsNameError()
    {
        var (_, nameError, _, _) = ProductValidationHelper.Validate("   ", "100", "Electronics");

        Assert.NotNull(nameError);
    }

    [Fact]
    public void Validate_WhenNameTooLong_ReturnsNameError()
    {
        var name = new string('a', 101);
        var (_, nameError, _, _) = ProductValidationHelper.Validate(name, "100", "Electronics");

        Assert.NotNull(nameError);
        Assert.Contains("100", nameError);
    }

    [Fact]
    public void Validate_WhenNameAtMaxLength_ReturnsNoError()
    {
        var name = new string('a', 100);
        var (isValid, nameError, _, _) = ProductValidationHelper.Validate(name, "100", "Electronics");

        Assert.True(isValid);
        Assert.Null(nameError);
    }

    [Fact]
    public void Validate_WhenPriceEmpty_ReturnsPriceError()
    {
        var (_, _, priceError, _) = ProductValidationHelper.Validate("Laptop", "", "Electronics");

        Assert.NotNull(priceError);
    }

    [Fact]
    public void Validate_WhenPriceNonNumeric_ReturnsPriceError()
    {
        var (_, _, priceError, _) = ProductValidationHelper.Validate("Laptop", "abc", "Electronics");

        Assert.NotNull(priceError);
        Assert.Contains("valid", priceError, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Validate_WhenPriceZero_ReturnsPriceError()
    {
        var (_, _, priceError, _) = ProductValidationHelper.Validate("Laptop", "0", "Electronics");

        Assert.NotNull(priceError);
        Assert.Contains("greater than zero", priceError, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Validate_WhenPriceNegative_ReturnsPriceError()
    {
        var (_, _, priceError, _) = ProductValidationHelper.Validate("Laptop", "-10", "Electronics");

        Assert.NotNull(priceError);
    }

    [Fact]
    public void Validate_WhenPriceTooHigh_ReturnsPriceError()
    {
        var (_, _, priceError, _) = ProductValidationHelper.Validate("Laptop", "1000001", "Electronics");

        Assert.NotNull(priceError);
    }

    [Fact]
    public void Validate_WhenPriceAtMax_ReturnsNoError()
    {
        var (isValid, _, priceError, _) = ProductValidationHelper.Validate("Laptop", "1000000", "Electronics");

        Assert.True(isValid);
        Assert.Null(priceError);
    }

    [Fact]
    public void Validate_WhenPriceJustAboveZero_ReturnsNoError()
    {
        var (isValid, _, priceError, _) = ProductValidationHelper.Validate("Laptop", "0.01", "Electronics");

        Assert.True(isValid);
        Assert.Null(priceError);
    }

    [Fact]
    public void Validate_WhenCategoryEmpty_ReturnsCategoryError()
    {
        var (_, _, _, categoryError) = ProductValidationHelper.Validate("Laptop", "100", "");

        Assert.NotNull(categoryError);
    }

    [Fact]
    public void Validate_WhenCategoryWhitespace_ReturnsCategoryError()
    {
        var (_, _, _, categoryError) = ProductValidationHelper.Validate("Laptop", "100", "   ");

        Assert.NotNull(categoryError);
    }

    [Fact]
    public void Validate_WhenCategoryTooLong_ReturnsCategoryError()
    {
        var category = new string('a', 51);
        var (_, _, _, categoryError) = ProductValidationHelper.Validate("Laptop", "100", category);

        Assert.NotNull(categoryError);
        Assert.Contains("50", categoryError);
    }

    [Fact]
    public void Validate_WhenCategoryAtMaxLength_ReturnsNoError()
    {
        var category = new string('a', 50);
        var (isValid, _, _, categoryError) = ProductValidationHelper.Validate("Laptop", "100", category);

        Assert.True(isValid);
        Assert.Null(categoryError);
    }

    [Fact]
    public void Validate_WhenMultipleErrors_ReturnsAllErrors()
    {
        var (isValid, nameError, priceError, categoryError) =
            ProductValidationHelper.Validate("", "-1", "");

        Assert.False(isValid);
        Assert.NotNull(nameError);
        Assert.NotNull(priceError);
        Assert.NotNull(categoryError);
    }

    [Fact]
    public void Validate_WhenPriceHasLeadingZeros_ParsesCorrectly()
    {
        var (isValid, priceError, _, _) =
            ProductValidationHelper.Validate("Laptop", "000001.00", "Electronics");

        Assert.True(isValid);
        Assert.Null(priceError);
    }

    [Fact]
    public void Validate_WhenPriceHasSpaces_ParsesCorrectly()
    {
        var (isValid, priceError, _, _) =
            ProductValidationHelper.Validate("Laptop", "  100  ", "Electronics");

        Assert.True(isValid);
        Assert.Null(priceError);
    }
}
