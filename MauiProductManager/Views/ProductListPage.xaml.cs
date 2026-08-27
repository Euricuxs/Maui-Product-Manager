using System.Globalization;
using System.ComponentModel;
using Microsoft.Maui.Controls.Shapes;
using MauiProductManager.ViewModels;

namespace MauiProductManager.Views;

public class ProductListPage : ContentPage
{
    private readonly ProductListViewModel _viewModel;

    public ProductListPage()
    {
        _viewModel = App.Services.GetRequiredService<ProductListViewModel>();
        BindingContext = _viewModel;
        Title = "Products";
        BackgroundColor = Colors.White;

        var searchBar = new SearchBar
        {
            Placeholder = "Search by name or category",
            Margin = new Thickness(12, 8, 12, 2),
            MinimumHeightRequest = 48,
            BackgroundColor = Colors.Transparent
        };
        searchBar.SetBinding(SearchBar.TextProperty, "SearchText", BindingMode.OneWay);
        searchBar.TextChanged += (s, e) => _viewModel.SetSearchText(e.NewTextValue ?? string.Empty);

        var collectionView = new CollectionView
        {
            BackgroundColor = Colors.White,
            SelectionMode = SelectionMode.None,
            Header = searchBar
        };
        collectionView.SetBinding(CollectionView.ItemsSourceProperty, "FilteredProducts");

        var dataTemplate = new DataTemplate(() =>
        {
            var border = new Border
            {
                BackgroundColor = Colors.White,
                StrokeThickness = 0,
                StrokeShape = new RoundRectangle { CornerRadius = 12 },
                Padding = 16,
                Margin = new Thickness(16, 6),
                Shadow = new Shadow
                {
                    Brush = Colors.LightGray,
                    Radius = 6,
                    Opacity = 0.3f,
                    Offset = new Point(0, 2)
                }
            };

            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += (s, e) =>
            {
                if (s is Border b && b.BindingContext is Models.Product product)
                {
                    _viewModel.CancelLoad();
                    Navigation.PushAsync(new ProductDetailPage(product.Id), false);
                }
            };
            border.GestureRecognizers.Add(tapGesture);

            var grid = new Grid
            {
                BackgroundColor = Colors.White,
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = GridLength.Auto }
                }
            };

            var leftStack = new VerticalStackLayout { Spacing = 4 };
            var nameLabel = new Label { FontSize = 17, FontAttributes = FontAttributes.Bold, TextColor = Color.FromRgb(33, 33, 33) };
            nameLabel.SetBinding(Label.TextProperty, "Name");
            var categoryLabel = new Label { FontSize = 13, TextColor = Color.FromRgb(110, 110, 110) };
            categoryLabel.SetBinding(Label.TextProperty, "Category");
            leftStack.Add(nameLabel);
            leftStack.Add(categoryLabel);

            var priceLabel = new Label { FontSize = 16, FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center, TextColor = Color.FromRgb(81, 43, 212) };
            priceLabel.SetBinding(Label.TextProperty, new Binding("Price", stringFormat: "${0:F2}"));

            grid.Add(leftStack, 0, 0);
            grid.Add(priceLabel, 1, 0);
            border.Content = grid;
            return border;
        });
        collectionView.ItemTemplate = dataTemplate;

        var footerStack = new VerticalStackLayout
        {
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Spacing = 8,
            Margin = new Thickness(0, 20)
        };
        var footerLabel = new Label { Text = "No products found.", FontSize = 16, HorizontalTextAlignment = TextAlignment.Center, TextColor = Color.FromRgb(110, 110, 110) };
        footerStack.Add(footerLabel);
        footerStack.SetBinding(VisualElement.IsVisibleProperty, "IsSearchResultEmpty");
        collectionView.Footer = footerStack;

        var refreshView = new RefreshView
        {
            Content = collectionView
        };
        refreshView.SetBinding(RefreshView.IsRefreshingProperty, "IsRefreshing");
        refreshView.SetBinding(RefreshView.CommandProperty, "RefreshCommand");
        refreshView.SetBinding(VisualElement.IsVisibleProperty, "HasContent");

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
        retryButton.SetBinding(Button.CommandProperty, "RefreshCommand");
        errorStack.Add(errorLabel);
        errorStack.Add(retryButton);
        errorStack.SetBinding(VisualElement.IsVisibleProperty, "HasError");

        var emptyStack = new VerticalStackLayout
        {
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Spacing = 8,
            Padding = 24
        };
        var emptyLabel = new Label { Text = "No products available.", FontSize = 16, HorizontalTextAlignment = TextAlignment.Center, TextColor = Color.Parse("#6E6E6E") };
        emptyStack.Add(emptyLabel);
        emptyStack.SetBinding(VisualElement.IsVisibleProperty, "HasContent", converter: new InverseBoolConverter());

        var contentGrid = new Grid
        {
            RowDefinitions = new RowDefinitionCollection
            {
                new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }
            }
        };
        contentGrid.Add(loadingIndicator, 0, 0);
        contentGrid.Add(errorStack, 0, 0);
        contentGrid.Add(emptyStack, 0, 0);
        contentGrid.Add(refreshView, 0, 0);

        Content = contentGrid;

        var toolbarItem = new ToolbarItem { Text = "+" };
        toolbarItem.SetBinding(ToolbarItem.CommandProperty, "GoToCreateCommand");
        ToolbarItems.Add(toolbarItem);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.StartRefresh();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.CancelLoad();
    }
}

public class InverseBoolConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool b)
            return !b;
        return value != null;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
