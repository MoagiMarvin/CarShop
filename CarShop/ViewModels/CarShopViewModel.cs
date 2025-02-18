using CarShop.Data;
using CarShop.Model;
using CarShop.ViewModels;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CarShop.ViewModels;

public class CarShopViewModel : BaseViewModel
{
    private ObservableCollection<Car> _cars;
    private Car _selectedCar;
    private string _statusMessage;
    private int _quantity = 1;

    public ObservableCollection<Car> Cars
    {
        get => _cars;
        set => SetProperty(ref _cars, value);
    }

    public Car SelectedCar
    {
        get => _selectedCar;
        set => SetProperty(ref _selectedCar, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public int Quantity
    {
        get => _quantity;
        set => SetProperty(ref _quantity, value);
    }

    public ICommand LoadCarsCommand { get; }
    public ICommand AddToCartCommand { get; }

    public CarShopViewModel(IDataService dataService) : base(dataService)
    {
        _cars = new ObservableCollection<Car>();
        LoadCarsCommand = new AsyncRelayCommand(LoadCarsAsync);
        AddToCartCommand = new AsyncRelayCommand(AddToCartAsync);
        LoadCarsCommand.Execute(null);
    }

    private async Task LoadCarsAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            var carList = await _dataService.GetAllCarsAsync();
            Cars.Clear();
            foreach (var car in carList)
            {
                Cars.Add(car);
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading cars: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task AddToCartAsync()
    {
        if (IsBusy || SelectedCar == null || Quantity <= 0) return;

        try
        {
            IsBusy = true;

            var profile = await _dataService.GetFirstProfileAsync();
            if (profile == null)
            {
                StatusMessage = "Please create a profile first";
                return;
            }

            var cartItem = new CartItem
            {
                ProfileId = profile.ProfileId,
                CarId = SelectedCar.CarId,
                Quantity = Quantity
            };

            await _dataService.AddToCartAsync(cartItem);
            StatusMessage = "Added to cart successfully!";

            await LoadCarsAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error adding to cart: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
