using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiProductManager.Models;
using MauiProductManager.Services;

namespace MauiProductManager.ViewModels;

public partial class CreateProductViewModel : ObservableObject
{
    private readonly IProductService _productService;

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _price = string.Empty;

    [ObservableProperty]
    private string _category = string.Empty;

    [ObservableProperty]
    private string? _nameError;

    [ObservableProperty]
    private string? _priceError;

    [ObservableProperty]
    private string? _categoryError;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _hasError;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public CreateProductViewModel(IProductService productService)
    {
        _productService = productService;
    }

    partial void OnNameChanged(string value) => NameError = null;
    partial void OnPriceChanged(string value) => PriceError = null;
    partial void OnCategoryChanged(string value) => CategoryError = null;

    [RelayCommand]
    private async Task Save()
    {
        if (IsLoading)
            return;

        HasError = false;
        ErrorMessage = string.Empty;

        var (isValid, nameError, priceError, categoryError) =
            ProductValidationHelper.Validate(Name, Price, Category);

        NameError = nameError;
        PriceError = priceError;
        CategoryError = categoryError;

        if (!isValid)
            return;

        var product = new Product
        {
            Name = Name.Trim(),
            Price = decimal.Parse(Price),
            Category = Category.Trim(),
        };

        IsLoading = true;

        try
        {
            await _productService.CreateProductAsync(product);
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception)
        {
            HasError = true;
            ErrorMessage = "Unable to create product. Please try again.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task Cancel()
    {
        await Shell.Current.GoToAsync("..");
    }
}
