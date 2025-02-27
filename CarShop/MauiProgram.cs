using CarShop.Data;
using CarShop.Data.Services;
using CarShop.ViewModels;
using CarShop.Views;
using Microsoft.Extensions.Logging;

namespace CarShop
{
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
            // Get the database path
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "carshops.db");

            // Create database context
            var databaseContext = new DatabaseContext(dbPath);

            // Initialize database
            Task.Run(async () =>
            {
                try
                {
                    await databaseContext.InitializeAsync();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error initializing database: {ex.Message}");
                }
            });

            // Register database context
            builder.Services.AddSingleton(databaseContext);
            builder.Services.AddSingleton<IDataService, DataService>();

            // Register ViewModels as singletons
            builder.Services.AddSingleton<CarShopViewModel>();
            builder.Services.AddSingleton<CartViewModel>();
            builder.Services.AddSingleton<ProfileViewModel>();

            // Register Pages as transients
            builder.Services.AddTransient<CarShopPage>();
            builder.Services.AddTransient<CartPage>();
            builder.Services.AddTransient<ProfilePage>();

            // Register Shell
            builder.Services.AddSingleton<AppShell>();
#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
