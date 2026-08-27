using System.Globalization;
using System.Net;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MauiProductManager.Models;
using MauiProductManager.Services;

namespace MauiProductManager.ViewModels;

public partial class EditProductViewModel : ObservableObject
{
    private readonly IProductService _productService;
    private int _productId;

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
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isLoadingProduct;

    public bool IsAnyLoading => IsLoadingProduct || IsLoading;

    [ObservableProperty]
    private bool _hasError;

    public EditProductViewModel(IProductService productService)
    {
        _productService = productService;
    }

    partial void OnNameChanged(string value) => NameError = null;
    partial void OnPriceChanged(string value) => PriceError = null;
    partial void OnCategoryChanged(string value) => CategoryError = null;
    partial void OnIsLoadingChanged(bool value) => OnPropertyChanged(nameof(IsAnyLoading));
    partial void OnIsLoadingProductChanged(bool value) => OnPropertyChanged(nameof(IsAnyLoading));

    public async Task LoadProductAsync(int productId, CancellationToken cancellationToken = default)
    {
        _productId = productId;
        IsLoadingProduct = true;
        HasError = false;
        ErrorMessage = string.Empty;

        try
        {
            var product = await _productService.GetProductAsync(productId, cancellationToken);
            if (cancellationToken.IsCancellationRequested)
                return;
            if (product != null)
            {
                Name = product.Name;
                Price = product.Price.ToString("F2", CultureInfo.InvariantCulture);
                Category = product.Category;
            }
            else
            {
                HasError = true;
                ErrorMessage = "The product could not be found. It may have been removed.";
            }
        }
        catch (OperationCanceledException)
        {
            // Cancelled, ignore silently
        }
        catch (HttpRequestException ex)
        {
            HasError = true;
            if (ex.Data.Contains("StatusCode") && (int)ex.Data["StatusCode"]! == 404)
            {
                ErrorMessage = "The product could not be found. It may have been removed.";
            }
            else if (ex.StatusCode.HasValue)
            {
                ErrorMessage = "Something went wrong. Please try again.";
            }
            else
            {
                ErrorMessage = "Unable to connect to the server. Please check your connection and try again.";
            }
        }
        catch (Exception)
        {
            HasError = true;
            ErrorMessage = "Something went wrong. Please try again.";
        }
        finally
        {
            IsLoadingProduct = false;
        }
    }

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

        IsLoading = true;

        try
        {
            var updatedProduct = new Product
            {
                Id = _productId,
                Name = Name.Trim(),
                Price = decimal.Parse(Price),
                Category = Category.Trim(),
            };

            await _productService.UpdateProductAsync(updatedProduct);
            WeakReferenceMessenger.Default.Send(new ProductUpdatedMessage(updatedProduct));
            await Shell.Current.GoToAsync("..");
        }
        catch (HttpRequestException ex)
        {
            HasError = true;
            if (ex.Data.Contains("StatusCode") && (int)ex.Data["StatusCode"]! == 404)
            {
                ErrorMessage = "The product could not be found. It may have been removed.";
            }
            else if (ex.StatusCode.HasValue)
            {
                ErrorMessage = "Something went wrong. Please try again.";
            }
            else
            {
                ErrorMessage = "Unable to connect to the server. Please check your connection and try again.";
            }
        }
        catch (Exception)
        {
            HasError = true;
            ErrorMessage = "Something went wrong. Please try again.";
        }
        finally
        {
            IsLoading = false;
        }
    }
}
