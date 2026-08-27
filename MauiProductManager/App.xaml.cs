using Microsoft.Extensions.DependencyInjection;

namespace MauiProductManager;

public partial class App : Application
{
	public static IServiceProvider Services { get; set; } = null!;

	public App()
	{
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new AppShell());
	}
}
