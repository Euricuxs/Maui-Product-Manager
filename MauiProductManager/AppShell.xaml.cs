using MauiProductManager.Views;

namespace MauiProductManager;

public partial class AppShell : Shell
{
	public AppShell()
	{
		Items.Add(new ShellContent
		{
			Title = "Products",
			Route = "MainPage",
			Content = new ProductListPage()
		});

		Routing.RegisterRoute("ProductDetailPage", typeof(ProductDetailPage));
		Routing.RegisterRoute("CreateProductPage", typeof(CreateProductPage));
		Routing.RegisterRoute("EditProductPage", typeof(EditProductPage));
	}
}
