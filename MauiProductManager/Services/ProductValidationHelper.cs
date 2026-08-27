namespace MauiProductManager.Services;

public static class ProductValidationHelper
{
    public const int MaxNameLength = 100;
    public const int MaxCategoryLength = 50;
    public const decimal MaxPrice = 1_000_000m;

    public static (bool isValid, string? nameError, string? priceError, string? categoryError)
        Validate(string name, string price, string category)
    {
        string? nameError = null;
        if (string.IsNullOrWhiteSpace(name))
            nameError = "Product name is required.";
        else if (name.Length > MaxNameLength)
            nameError = $"Product name must be {MaxNameLength} characters or fewer.";

        string? priceError = null;
        if (string.IsNullOrWhiteSpace(price))
            priceError = "Price is required.";
        else if (!decimal.TryParse(price, out var parsedPrice))
            priceError = "Price must be a valid number.";
        else if (parsedPrice <= 0)
            priceError = "Price must be greater than zero.";
        else if (parsedPrice > MaxPrice)
            priceError = $"Price must be {MaxPrice:C} or less.";

        string? categoryError = null;
        if (string.IsNullOrWhiteSpace(category))
            categoryError = "Category is required.";
        else if (category.Length > MaxCategoryLength)
            categoryError = $"Category must be {MaxCategoryLength} characters or fewer.";

        return (nameError == null && priceError == null && categoryError == null,
                nameError, priceError, categoryError);
    }
}
