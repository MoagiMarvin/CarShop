using CarShop.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarShop.Data.Services
{
    public class DataService : IDataService
    {
        private readonly DatabaseContext _context;

        public DataService(DatabaseContext context)
        {
            _context = context;
        }

        private async Task EnsureDatabaseInitializedAsync()
        {
            if (!_context.IsInitialized)
            {
                await _context.InitializeAsync();
            }
        }

        // Profile Methods
        public async Task<Profile> GetProfileAsync(int id)
        {
            await EnsureDatabaseInitializedAsync();
            return await _context.Database.Table<Profile>()
                .Where(p => p.ProfileId == id)
                .FirstOrDefaultAsync();
        }

        public async Task<Profile> GetFirstProfileAsync()
        {
            await EnsureDatabaseInitializedAsync();
            return await _context.Database.Table<Profile>()
                .FirstOrDefaultAsync();
        }

        public async Task<int> SaveProfileAsync(Profile profile)
        {
            await EnsureDatabaseInitializedAsync();
            if (profile.ProfileId == 0)
            {
                return await _context.Database.InsertAsync(profile);
            }
            return await _context.Database.UpdateAsync(profile);
        }

        // Car Methods
        public async Task<List<Car>> GetAllCarsAsync()
        {
            await EnsureDatabaseInitializedAsync();
            return await _context.Database.Table<Car>().ToListAsync();
        }

        public async Task<Car> GetCarAsync(int id)
        {
            await EnsureDatabaseInitializedAsync();
            return await _context.Database.Table<Car>()
                .Where(c => c.CarId == id)
                .FirstOrDefaultAsync();
        }

        public async Task<int> UpdateCarStockAsync(int carId, int newQuantity)
        {
            await EnsureDatabaseInitializedAsync();
            var car = await GetCarAsync(carId);
            if (car != null)
            {
                car.StockQuantity = newQuantity;
                return await _context.Database.UpdateAsync(car);
            }
            return 0;
        }

        // Cart Methods
        public async Task<List<CartItem>> GetCartItemsAsync(int profileId)
        {
            await EnsureDatabaseInitializedAsync();
            var cartItems = await _context.Database.Table<CartItem>()
                .Where(c => c.ProfileId == profileId)
                .ToListAsync();

            // Load associated car data
            foreach (var item in cartItems)
            {
                item.Car = await GetCarAsync(item.CarId);
            }

            return cartItems;
        }

        public async Task<int> AddToCartAsync(CartItem item)
        {
            await EnsureDatabaseInitializedAsync();
            if (await IsStockAvailableAsync(item.CarId, item.Quantity))
            {
                item.AddedDate = DateTime.Now;
                var car = await GetCarAsync(item.CarId);
                item.PriceAtTime = car.Price;

                // Update stock quantity
                await UpdateCarStockAsync(item.CarId, car.StockQuantity - item.Quantity);

                return await _context.Database.InsertAsync(item);
            }
            throw new InvalidOperationException("Requested quantity exceeds available stock");
        }

        public async Task<int> RemoveFromCartAsync(int cartItemId)
        {
            await EnsureDatabaseInitializedAsync();
            var item = await _context.Database.Table<CartItem>()
                .Where(c => c.CartItemId == cartItemId)
                .FirstOrDefaultAsync();

            if (item != null)
            {
                var car = await GetCarAsync(item.CarId);
                // Restore stock quantity
                await UpdateCarStockAsync(item.CarId, car.StockQuantity + item.Quantity);

                return await _context.Database.DeleteAsync(item);
            }
            return 0;
        }

        public async Task<int> UpdateCartItemQuantityAsync(int cartItemId, int quantity)
        {
            await EnsureDatabaseInitializedAsync();
            var item = await _context.Database.Table<CartItem>()
                .Where(c => c.CartItemId == cartItemId)
                .FirstOrDefaultAsync();

            if (item != null)
            {
                var car = await GetCarAsync(item.CarId);
                int quantityDifference = quantity - item.Quantity;

                if (await IsStockAvailableAsync(item.CarId, quantityDifference))
                {
                    // Update stock quantity
                    await UpdateCarStockAsync(item.CarId, car.StockQuantity - quantityDifference);

                    item.Quantity = quantity;
                    return await _context.Database.UpdateAsync(item);
                }
                throw new InvalidOperationException("Requested quantity exceeds available stock");
            }
            return 0;
        }

        public async Task<bool> IsStockAvailableAsync(int carId, int requestedQuantity)
        {
            await EnsureDatabaseInitializedAsync();
            var car = await GetCarAsync(carId);
            return car != null && car.StockQuantity >= requestedQuantity;
        }

        public async Task ClearCartAsync(int profileId)
        {
            await EnsureDatabaseInitializedAsync();
            var cartItems = await GetCartItemsAsync(profileId);
            foreach (var item in cartItems)
            {
                await RemoveFromCartAsync(item.CartItemId);
            }
        }
    }
}