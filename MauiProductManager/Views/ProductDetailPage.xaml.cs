using System.Globalization;
using MauiProductManager.ViewModels;

namespace MauiProductManager.Views;

public class ProductDetailPage : ContentPage, IQueryAttributable
{
    private readonly ProductDetailViewModel _viewModel;
    private CancellationTokenSource? _cts;
    private bool _hasLoadedOnce;

    public ProductDetailPage(ProductDetailViewModel viewModel)
    {
        _viewModel = viewModel;
        BindingContext = _viewModel;
        Title = "Product Details";
        BuildUI();
    }

    public ProductDetailPage(int productId)
    {
        var services = App.Services;
        _viewModel = services.GetRequiredService<ProductDetailViewModel>();
        BindingContext = _viewModel;
        Title = "Product Details";
        BuildUI();
        _ = _viewModel.LoadProductAsync(productId);
        _hasLoadedOnce = true;
    }

    private void BuildUI()
    {
        var loadingIndicator = new ActivityIndicator
        {
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };
        loadingIndicator.SetBinding(ActivityIndicator.IsRunningProperty, "IsLoading");
        loadingIndicator.SetBinding(ActivityIndicator.IsVisibleProperty, "IsLoading");

        var errorStack = new VerticalStackLayout
        {
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Spacing = 16,
            Padding = 24
        };
        var errorLabel = new Label { FontSize = 15, HorizontalTextAlignment = TextAlignment.Center, TextColor = Color.Parse("#DC3545") };
        errorLabel.SetBinding(Label.TextProperty, "ErrorMessage");
        var retryButton = new Button { Text = "Retry", HorizontalOptions = LayoutOptions.Center };
        retryButton.SetBinding(Button.CommandProperty, "LoadProductCommand");
        errorStack.Add(errorLabel);
        errorStack.Add(retryButton);
        errorStack.SetBinding(VisualElement.IsVisibleProperty, "HasError");

        var nameLabel = new Label { FontSize = 26, FontAttributes = FontAttributes.Bold };
        nameLabel.SetBinding(Label.TextProperty, "Product.Name");

        var separator = new BoxView { HeightRequest = 1, Margin = new Thickness(0, 4, 0, 4) };

        var priceStack = new VerticalStackLayout { Spacing = 4 };
        priceStack.Add(new Label { Text = "Price", FontSize = 12 });
        var priceValueLabel = new Label { FontSize = 24, FontAttributes = FontAttributes.Bold };
        priceValueLabel.SetBinding(Label.TextProperty, new Binding("Product.Price", stringFormat: "${0:F2}"));
        priceStack.Add(priceValueLabel);

        var categoryStack = new VerticalStackLayout { Spacing = 4 };
        categoryStack.Add(new Label { Text = "Category", FontSize = 12 });
        var categoryValueLabel = new Label { FontSize = 17 };
        categoryValueLabel.SetBinding(Label.TextProperty, "Product.Category");
        categoryStack.Add(categoryValueLabel);

        var cardContent = new VerticalStackLayout { Spacing = 16 };
        cardContent.Add(nameLabel);
        cardContent.Add(separator);
        cardContent.Add(priceStack);
        cardContent.Add(categoryStack);

        var card = new Frame
        {
            BackgroundColor = Colors.White,
            CornerRadius = 16,
            Padding = 20,
            Margin = 16,
            HasShadow = true,
            BorderColor = Colors.Transparent,
            Content = cardContent,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center
        };

        var scrollView = new ScrollView
        {
            Content = card,
            VerticalScrollBarVisibility = ScrollBarVisibility.Never
        };

        var deleteErrorLabel = new Label { FontSize = 12, TextColor = Color.Parse("#DC3545"), HorizontalTextAlignment = TextAlignment.Center };
        deleteErrorLabel.SetBinding(Label.TextProperty, "ErrorMessage");
        deleteErrorLabel.SetBinding(VisualElement.IsVisibleProperty, "HasError");

        var deleteButton = new Button { Text = "Delete Product", HorizontalOptions = LayoutOptions.Fill };
        deleteButton.SetBinding(Button.CommandProperty, "DeleteCommand");
        deleteButton.SetBinding(Button.IsEnabledProperty, "IsDeleting", converter: new InverseBoolConverter());

        var bottomStack = new VerticalStackLayout { Padding = 16, Spacing = 8 };
        bottomStack.Add(deleteErrorLabel);
        bottomStack.Add(deleteButton);

        var contentGrid = new Grid { VerticalOptions = LayoutOptions.Fill };
        contentGrid.Add(scrollView, 0, 0);

        var bottomBar = new VerticalStackLayout();
        bottomBar.Add(bottomStack);

        var outerGrid = new Grid();
        outerGrid.Add(loadingIndicator, 0, 0);
        outerGrid.Add(errorStack, 0, 0);

        var productGrid = new Grid { VerticalOptions = LayoutOptions.Fill };
        productGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        productGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        productGrid.Add(contentGrid, 0, 0);
        productGrid.Add(bottomBar, 0, 1);
        productGrid.SetBinding(VisualElement.IsVisibleProperty, "HasProduct");

        outerGrid.Add(productGrid, 0, 0);
        Content = outerGrid;

        var editToolbarItem = new ToolbarItem { Text = "Edit" };
        editToolbarItem.SetBinding(ToolbarItem.CommandProperty, "GoToEditCommand");
        editToolbarItem.SetBinding(ToolbarItem.IsEnabledProperty, "IsDeleting", converter: new InverseBoolConverter());
        ToolbarItems.Add(editToolbarItem);
    }

    void IQueryAttributable.ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("productId", out var value) && int.TryParse(value?.ToString(), out var productId))
        {
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            _ = _viewModel.LoadProductAsync(productId, _cts.Token);
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _cts?.Cancel();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (_hasLoadedOnce)
            return;
        if (_viewModel.ProductId > 0)
        {
            _ = _viewModel.LoadProductAsync(_viewModel.ProductId);
        }
        _hasLoadedOnce = true;
    }
}
