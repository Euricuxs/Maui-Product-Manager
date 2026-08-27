using System.Globalization;
using MauiProductManager.ViewModels;

namespace MauiProductManager.Views;

public class EditProductPage : ContentPage, IQueryAttributable
{
    private readonly EditProductViewModel _viewModel;
    private CancellationTokenSource? _cts;

    public EditProductPage(EditProductViewModel viewModel)
    {
        _viewModel = viewModel;
        BindingContext = _viewModel;
        Title = "Edit Product";

        var loadingIndicator = new ActivityIndicator
        {
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };
        loadingIndicator.SetBinding(ActivityIndicator.IsRunningProperty, "IsAnyLoading");
        loadingIndicator.SetBinding(ActivityIndicator.IsVisibleProperty, "IsAnyLoading");

        var nameLabel = new Label { Text = "Product Name", FontSize = 14, FontAttributes = FontAttributes.Bold };
        var nameEntry = new Entry { Placeholder = "Enter product name", MinimumHeightRequest = 48, FontSize = 15 };
        nameEntry.SetBinding(Entry.TextProperty, "Name", BindingMode.TwoWay);
        var nameErrorLabel = new Label { FontSize = 12, TextColor = Color.Parse("#DC3545"), Margin = new Thickness(0, 4, 0, 0) };
        nameErrorLabel.SetBinding(Label.TextProperty, "NameError");
        nameErrorLabel.SetBinding(VisualElement.IsVisibleProperty, "NameError", converter: new StringNotEmptyConverter());

        var priceLabel = new Label { Text = "Price", FontSize = 14, FontAttributes = FontAttributes.Bold };
        var priceEntry = new Entry { Placeholder = "0.00", Keyboard = Keyboard.Numeric, MinimumHeightRequest = 48, FontSize = 15 };
        priceEntry.SetBinding(Entry.TextProperty, "Price", BindingMode.TwoWay);
        var priceErrorLabel = new Label { FontSize = 12, TextColor = Color.Parse("#DC3545"), Margin = new Thickness(0, 4, 0, 0) };
        priceErrorLabel.SetBinding(Label.TextProperty, "PriceError");
        priceErrorLabel.SetBinding(VisualElement.IsVisibleProperty, "PriceError", converter: new StringNotEmptyConverter());

        var categoryLabel = new Label { Text = "Category", FontSize = 14, FontAttributes = FontAttributes.Bold };
        var categoryEntry = new Entry { Placeholder = "Enter category", MinimumHeightRequest = 48, FontSize = 15 };
        categoryEntry.SetBinding(Entry.TextProperty, "Category", BindingMode.TwoWay);
        var categoryErrorLabel = new Label { FontSize = 12, TextColor = Color.Parse("#DC3545"), Margin = new Thickness(0, 4, 0, 0) };
        categoryErrorLabel.SetBinding(Label.TextProperty, "CategoryError");
        categoryErrorLabel.SetBinding(VisualElement.IsVisibleProperty, "CategoryError", converter: new StringNotEmptyConverter());

        var errorLabel = new Label { FontSize = 12, TextColor = Color.Parse("#DC3545") };
        errorLabel.SetBinding(Label.TextProperty, "ErrorMessage");
        errorLabel.SetBinding(VisualElement.IsVisibleProperty, "HasError");

        var saveButton = new Button { Text = "Save Changes", Margin = new Thickness(0, 8, 0, 0) };
        saveButton.SetBinding(Button.CommandProperty, "SaveCommand");
        saveButton.SetBinding(Button.IsEnabledProperty, "IsAnyLoading", converter: new InverseBoolConverter());

        var formStack = new VerticalStackLayout { Padding = 20, Spacing = 20 };
        formStack.Add(nameLabel);
        formStack.Add(nameEntry);
        formStack.Add(nameErrorLabel);
        formStack.Add(priceLabel);
        formStack.Add(priceEntry);
        formStack.Add(priceErrorLabel);
        formStack.Add(categoryLabel);
        formStack.Add(categoryEntry);
        formStack.Add(categoryErrorLabel);
        formStack.Add(errorLabel);
        formStack.Add(saveButton);

        var scrollView = new ScrollView { Content = formStack, VerticalScrollBarVisibility = ScrollBarVisibility.Never };
        scrollView.SetBinding(VisualElement.IsEnabledProperty, "IsAnyLoading", converter: new InverseBoolConverter());

        var grid = new Grid();
        grid.Add(loadingIndicator, 0, 0);
        grid.Add(scrollView, 0, 0);
        Content = grid;
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
}
