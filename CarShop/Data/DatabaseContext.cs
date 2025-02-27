using CarShop.Model;
using SQLite;
using System.Collections.ObjectModel;

namespace CarShop.Data
{
    public class DatabaseContext : IDisposable
    {
        private readonly SQLiteAsyncConnection _database;
        private bool _isInitialized = false;
        private readonly SemaphoreSlim _initializationSemaphore = new SemaphoreSlim(1, 1);

        public DatabaseContext(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            System.Diagnostics.Debug.WriteLine($"Creating database at: {dbPath}");
        }

        public async Task InitializeAsync()
        {
            // Use a semaphore to prevent multiple concurrent initialization attempts
            await _initializationSemaphore.WaitAsync();

            try
            {
                if (_isInitialized)
                    return;

                System.Diagnostics.Debug.WriteLine("Starting database initialization");

                await _database.CreateTableAsync<Profile>();
                await _database.CreateTableAsync<Car>();
                await _database.CreateTableAsync<CartItem>();

                // Seed initial car data if the table is empty
                if (await _database.Table<Car>().CountAsync() == 0)
                {
                    System.Diagnostics.Debug.WriteLine("Seeding initial car data");
                    var cars = new List<Car>
                    {
                        new Car
                        {
                            Make = "Toyota",
                            Model = "Camry",
                            Year = 2024,
                            Color = "Silver",
                            Price = 25000,
                            StockQuantity = 5,
                            Description = "Reliable family sedan",
                            ImageUrl = "camry.png"
                        },
                        new Car
                        {
                            Make = "Honda",
                            Model = "CR-V",
                            Year = 2024,
                            Color = "Blue",
                            Price = 28000,
                            StockQuantity = 3,
                            Description = "Popular compact SUV",
                            ImageUrl = "cvc.jpeg"
                        },
                           new Car
    {
        Make = "Chevrolet",
        Model = "Silverado",
        Year = 2024,
        Color = "White",
        Price = 38000,
        StockQuantity = 6,
        Description = "Full-size pickup truck",
        ImageUrl = "silverado.png"
    },
    new Car
    {
        Make = "BMW",
        Model = "X5",
        Year = 2024,
        Color = "Gray",
        Price = 62000,
        StockQuantity = 2,
        Description = "Luxury SUV with advanced features",
        ImageUrl = "bmw_x5.png"
    },
    new Car
    {
        Make = "Audi",
        Model = "A4",
        Year = 2024,
        Color = "Silver",
        Price = 45000,
        StockQuantity = 3,
        Description = "Premium compact sedan",
        ImageUrl = "audi_a4.png"
    },
                    };
                    await _database.InsertAllAsync(cars);
                }

                _isInitialized = true;
                System.Diagnostics.Debug.WriteLine("Database initialization completed");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing database: {ex.Message}");
                throw;
            }
            finally
            {
                _initializationSemaphore.Release();
            }
        }

        public SQLiteAsyncConnection Database
        {
            get
            {
                if (!_isInitialized)
                    throw new InvalidOperationException("Database not initialized. Call InitializeAsync first.");

                return _database;
            }
        }

        public bool IsInitialized => _isInitialized;

        public void Dispose()
        {
            _database?.CloseAsync().Wait();
            _initializationSemaphore?.Dispose();
        }
    }
}