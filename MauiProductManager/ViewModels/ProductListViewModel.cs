using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiProductManager.Models;
using MauiProductManager.Services;

namespace MauiProductManager.ViewModels;

public partial class ProductListViewModel : ObservableObject
{
    private readonly IProductService _productService;
    private CancellationTokenSource? _loadCts;
    private CancellationTokenSource? _searchCts;

    [ObservableProperty]
    private ObservableCollection<Product> _products = [];

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _isRefreshing;

    [ObservableProperty]
    private bool _hasContent;

    [ObservableProperty]
    private bool _hasError;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private ObservableCollection<Product> _filteredProducts = [];

    [ObservableProperty]
    private int _itemCount;

    [ObservableProperty]
    private bool _hasFilteredContent;

    public bool IsSearchResultEmpty => HasContent && !HasFilteredContent;

    public ProductListViewModel(IProductService productService)
    {
        _productService = productService;
    }

    public void CancelLoad()
    {
        _loadCts?.Cancel();
    }

    public void StartRefresh()
    {
        CancelLoad();
        _loadCts = new CancellationTokenSource();
        if (!IsLoading && Products.Count > 0)
        {
            IsRefreshing = true;
        }
        _ = LoadProductsAsync(_loadCts.Token);
    }

    private async Task LoadProductsAsync(CancellationToken cancellationToken = default)
    {
        bool isFirstLoad = Products.Count == 0 && !IsRefreshing;

        if (IsLoading)
            return;

        if (isFirstLoad)
            IsLoading = true;

        ErrorMessage = string.Empty;
        HasError = false;

        try
        {
            var products = await _productService.GetProductsAsync(cancellationToken);
            if (cancellationToken.IsCancellationRequested)
                return;
            Products = products;
            HasContent = Products.Count > 0;
            ApplyFilter();
        }
        catch (Exception)
        {
            ErrorMessage = "Failed to load products. Please try again.";
            HasError = true;
        }
        finally
        {
            if (isFirstLoad)
                IsLoading = false;
            IsRefreshing = false;
        }
    }

    public void SetSearchText(string text)
    {
        _searchCts?.Cancel();
        _searchCts = new CancellationTokenSource();
        var token = _searchCts.Token;

        Task.Run(async () =>
        {
            try
            {
                await Task.Delay(300, token);
                if (!token.IsCancellationRequested)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        SearchText = text;
                    });
                }
            }
            catch (TaskCanceledException) { }
        }, token);
    }

    partial void OnSearchTextChanged(string value)
    {
        ApplyFilter();
    }

    partial void OnProductsChanged(ObservableCollection<Product> value)
    {
        ApplyFilter();
    }

    partial void OnHasContentChanged(bool value)
    {
        OnPropertyChanged(nameof(IsSearchResultEmpty));
    }

    private void ApplyFilter()
    {
        _filteredProducts.Clear();
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            foreach (var p in Products)
                _filteredProducts.Add(p);
        }
        else
        {
            var term = SearchText.Trim().ToUpperInvariant();
            foreach (var p in Products)
            {
                if (p.Name.ToUpperInvariant().Contains(term) ||
                    p.Category.ToUpperInvariant().Contains(term))
                    _filteredProducts.Add(p);
            }
        }
        HasFilteredContent = _filteredProducts.Count > 0;
        ItemCount = _filteredProducts.Count;
    }

    [RelayCommand]
    private async Task Refresh()
    {
        await LoadProductsAsync();
    }

    [RelayCommand]
    private async Task GoToCreate()
    {
        await Shell.Current.GoToAsync("CreateProductPage");
    }

    [RelayCommand]
    private void NavigateToDetail(Product product)
    {
        if (product == null)
            return;
        CancelLoad();
        Shell.Current.GoToAsync($"ProductDetailPage?productId={product.Id}");
    }
}
