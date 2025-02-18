using System.Collections.ObjectModel;
using CarShop.Data;
using System.Windows.Input;
using CarShop.Model;
using CommunityToolkit.Mvvm.Input;

namespace CarShop.ViewModels
{
    public class CartViewModel : BaseViewModel
{
    private ObservableCollection<CartItem> _cartItems;
    private decimal _totalAmount;
    private string _statusMessage;

    public ObservableCollection<CartItem> CartItems
    {
        get => _cartItems;
        set => SetProperty(ref _cartItems, value);
    }

    public decimal TotalAmount
    {
        get => _totalAmount;
        set => SetProperty(ref _totalAmount, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public ICommand LoadCartCommand { get; }
    public ICommand RemoveFromCartCommand { get; }
    public ICommand UpdateQuantityCommand { get; }
    public ICommand ClearCartCommand { get; }

    public CartViewModel(IDataService dataService) : base(dataService)
    {
        _cartItems = new ObservableCollection<CartItem>();
        LoadCartCommand = new AsyncRelayCommand(LoadCartAsync);
        RemoveFromCartCommand = new AsyncRelayCommand<CartItem>(RemoveFromCartAsync);
        UpdateQuantityCommand = new AsyncRelayCommand<CartItem>(UpdateQuantityAsync);
        ClearCartCommand = new AsyncRelayCommand(ClearCartAsync);
        LoadCartCommand.Execute(null);
    }

    private async Task LoadCartAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;

            var profile = await _dataService.GetFirstProfileAsync();
            if (profile == null)
            {
                StatusMessage = "Please create a profile first";
                return;
            }

            var items = await _dataService.GetCartItemsAsync(profile.ProfileId);
            CartItems.Clear();
            foreach (var item in items)
            {
                CartItems.Add(item);
            }

            CalculateTotal();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading cart: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task RemoveFromCartAsync(CartItem item)
    {
        if (IsBusy || item == null) return;

        try
        {
            IsBusy = true;
            await _dataService.RemoveFromCartAsync(item.CartItemId);
            CartItems.Remove(item);
            CalculateTotal();
            StatusMessage = "Item removed from cart";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error removing item: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task UpdateQuantityAsync(CartItem item)
    {
        if (IsBusy || item == null) return;

        try
        {
            IsBusy = true;
            await _dataService.UpdateCartItemQuantityAsync(item.CartItemId, item.Quantity);
            CalculateTotal();
            StatusMessage = "Quantity updated";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error updating quantity: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ClearCartAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            var profile = await _dataService.GetFirstProfileAsync();
            if (profile != null)
            {
                await _dataService.ClearCartAsync(profile.ProfileId);
                CartItems.Clear();
                TotalAmount = 0;
                StatusMessage = "Cart cleared";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error clearing cart: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void CalculateTotal()
    {
        TotalAmount = CartItems.Sum(item => item.Quantity * item.PriceAtTime);
    }
}
}