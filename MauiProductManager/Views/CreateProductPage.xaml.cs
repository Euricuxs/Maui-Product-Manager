using System.Globalization;
using MauiProductManager.ViewModels;

namespace MauiProductManager.Views;

public class CreateProductPage : ContentPage
{
    public CreateProductPage()
    {
        BindingContext = App.Services.GetRequiredService<CreateProductViewModel>();
        Title = "Create Product";

        var loadingIndicator = new ActivityIndicator
        {
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };
        loadingIndicator.SetBinding(ActivityIndicator.IsRunningProperty, "IsLoading");
        loadingIndicator.SetBinding(ActivityIndicator.IsVisibleProperty, "IsLoading");

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

        var saveButton = new Button { Text = "Save Product", Margin = new Thickness(0, 8, 0, 0) };
        saveButton.SetBinding(Button.CommandProperty, "SaveCommand");
        saveButton.SetBinding(Button.IsEnabledProperty, "IsLoading", converter: new InverseBoolConverter());

        var errorLabel = new Label { FontSize = 12, TextColor = Color.Parse("#DC3545") };
        errorLabel.SetBinding(Label.TextProperty, "ErrorMessage");
        errorLabel.SetBinding(VisualElement.IsVisibleProperty, "HasError");

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
        scrollView.SetBinding(VisualElement.IsEnabledProperty, "IsLoading", converter: new InverseBoolConverter());

        var grid = new Grid();
        grid.Add(loadingIndicator, 0, 0);
        grid.Add(scrollView, 0, 0);
        Content = grid;
    }
}

public class StringNotEmptyConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => !string.IsNullOrEmpty(value?.ToString());

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
