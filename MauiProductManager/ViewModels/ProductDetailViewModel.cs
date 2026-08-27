using System.Net;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MauiProductManager.Models;
using MauiProductManager.Services;

namespace MauiProductManager.ViewModels;

public partial class ProductDetailViewModel : ObservableObject,
    IRecipient<ProductUpdatedMessage>,
    IRecipient<ProductDeletedMessage>
{
    private readonly IProductService _productService;
    private readonly Func<string, string, string, string, Task<bool>> _displayConfirmation;
    private CancellationTokenSource? _loadCts;

    [ObservableProperty]
    private Product? _product;

    [ObservableProperty]
    private int _productId;

    [ObservableProperty]
    private bool _isDeleting;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _hasError;

    [ObservableProperty]
    private bool _hasProduct;

    public ProductDetailViewModel(
        IProductService productService,
        Func<string, string, string, string, Task<bool>>? displayConfirmation = null)
    {
        _productService = productService;
        _displayConfirmation = displayConfirmation
            ?? (async (title, message, accept, cancel) =>
            {
                var page = Application.Current?.Windows?[0]?.Page;
                return page != null
                    ? await page.DisplayAlertAsync(title, message, accept, cancel)
                    : false;
            });

        WeakReferenceMessenger.Default.RegisterAll(this);
    }

    public void Receive(ProductUpdatedMessage message)
    {
        if (Product != null && Product.Id == message.Product.Id)
        {
            Product = message.Product;
        }
    }

    public void Receive(ProductDeletedMessage message)
    {
        if (Product != null && Product.Id == message.ProductId)
        {
            Shell.Current.GoToAsync("//MainPage");
        }
    }

    public async Task LoadProductAsync(int productId, CancellationToken cancellationToken = default)
    {
        _loadCts?.Cancel();
        _loadCts = new CancellationTokenSource();
        var linkedToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _loadCts.Token);
        ProductId = productId;
        IsLoading = true;
        HasError = false;
        ErrorMessage = string.Empty;
        Product = null;
        HasProduct = false;

        try
        {
            linkedToken.Token.ThrowIfCancellationRequested();
            Product = await _productService.GetProductAsync(productId, linkedToken.Token);
            HasProduct = Product != null;
        }
        catch (OperationCanceledException)
        {
            Product = null;
            HasProduct = false;
        }
        catch (HttpRequestException ex)
        {
            Product = null;
            HasProduct = false;
            HasError = true;

            if (ex.Data.Contains("StatusCode"))
            {
                var statusCode = (int)ex.Data["StatusCode"]!;
                if (statusCode == 404)
                {
                    ErrorMessage = "The product could not be found.";
                }
                else if (statusCode >= 500)
                {
                    ErrorMessage = "Server error. Please try again later.";
                }
                else
                {
                    ErrorMessage = "Unable to load product. Please try again.";
                }
            }
            else
            {
                ErrorMessage = "Unable to connect to the server. Please check your connection.";
            }
        }
        catch (Exception)
        {
            Product = null;
            HasProduct = false;
            HasError = true;
            ErrorMessage = "Something went wrong. Please try again.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task LoadProduct()
    {
        await LoadProductAsync(ProductId);
    }

    [RelayCommand]
    private async Task GoToEdit()
    {
        if (Product != null)
        {
            await Shell.Current.GoToAsync($"EditProductPage?productId={Product.Id}");
        }
    }

    [RelayCommand]
    private async Task Delete()
    {
        if (IsDeleting || Product == null)
            return;

        var confirmed = await _displayConfirmation(
            "Delete Product",
            $"Are you sure you want to delete '{Product.Name}'?",
            "Delete",
            "Cancel");

        if (!confirmed)
            return;

        IsDeleting = true;
        HasError = false;
        ErrorMessage = string.Empty;

        try
        {
            await _productService.DeleteProductAsync(Product.Id);
            await Shell.Current.GoToAsync("//MainPage");
        }
        catch (HttpRequestException ex) when (ex.Data.Contains("StatusCode") && (int)ex.Data["StatusCode"]! == 404)
        {
            HasError = true;
            ErrorMessage = "The product could not be found.";
        }
        catch (HttpRequestException ex) when (ex.Data.Contains("StatusCode") && (int)ex.Data["StatusCode"]! >= 500)
        {
            HasError = true;
            ErrorMessage = "Server error. Please try again later.";
        }
        catch (HttpRequestException)
        {
            HasError = true;
            ErrorMessage = "Unable to connect to the server. Please check your connection.";
        }
        catch (Exception)
        {
            HasError = true;
            ErrorMessage = "Something went wrong. Please try again.";
        }
        finally
        {
            IsDeleting = false;
        }
    }
}
