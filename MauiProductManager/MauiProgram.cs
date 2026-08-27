using System.Reflection;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MauiProductManager.Services;
using MauiProductManager.ViewModels;

namespace MauiProductManager;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        var apiBaseUrl = LoadApiBaseUrl();

        builder.Services.AddSingleton(sp => new HttpClient { BaseAddress = new Uri(apiBaseUrl) });
        builder.Services.AddSingleton<IProductService>(sp =>
            new ProductService(sp.GetRequiredService<HttpClient>()));

        builder.Services.AddTransient<ProductListViewModel>();
        builder.Services.AddTransient<ProductDetailViewModel>();
        builder.Services.AddTransient<CreateProductViewModel>();
        builder.Services.AddTransient<EditProductViewModel>();

        var mauiApp = builder.Build();
        App.Services = mauiApp.Services;
        return mauiApp;
    }

    private static string LoadApiBaseUrl()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = "MauiProductManager.appsettings.json";

        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream != null)
        {
            using var reader = new StreamReader(stream);
            var json = reader.ReadToEnd();
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty("ApiBaseUrl", out var urlElement))
            {
                return urlElement.GetString() ?? DefaultApiBaseUrl;
            }
        }

        return DefaultApiBaseUrl;
    }

    private const string DefaultApiBaseUrl = "http://10.0.2.2:5000";
}
