using CarShop.Model;

namespace CarShop.Data
{
    public interface IDataService
    {
        // Profile operations
        Task<Profile> GetProfileAsync(int id);
        Task<Profile> GetFirstProfileAsync();
        Task<int> SaveProfileAsync(Profile profile);

        // Car operations
        Task<List<Car>> GetAllCarsAsync();
        Task<Car> GetCarAsync(int id);
        Task<int> UpdateCarStockAsync(int carId, int newQuantity);

        // Cart operations
        Task<List<CartItem>> GetCartItemsAsync(int profileId);
        Task<int> AddToCartAsync(CartItem item);
        Task<int> RemoveFromCartAsync(int cartItemId);
        Task<int> UpdateCartItemQuantityAsync(int cartItemId, int quantity);
        Task<bool> IsStockAvailableAsync(int carId, int requestedQuantity);
        Task ClearCartAsync(int profileId);
    }
}
