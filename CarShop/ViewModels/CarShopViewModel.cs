using CarShop.Data;
using CarShop.Model;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CarShop.ViewModels
{
    public partial class CarShopViewModel : BaseViewModel
    {
        private ObservableCollection<Car> _cars;
        private Car _selectedCar;
        private string _statusMessage;
        private int _quantity = 1;
        private string _imageUrl;

        public ObservableCollection<Car> Cars
        {
            get => _cars;
            set => SetProperty(ref _cars, value);
        }
        public string ImageUrl
        {
            get => _imageUrl;
            set
            {
                _imageUrl = value;
                OnPropertyChanged(nameof(ImageUrl));
            }
        }
        public Car SelectedCar
        {
            get => _selectedCar;
            set
            {
                try
                {
                    System.Diagnostics.Debug.WriteLine($"SelectedCar changed to: {(value != null ? $"{value.Make} {value.Model}" : "null")}");
                    SetProperty(ref _selectedCar, value);

                    // Remove this line that's causing the exception:
                    // ((Command)AddToCartCommand).ChangeCanExecute();

                    // Instead, if using AsyncRelayCommand, you need to handle this differently
                    // Let the commanding system handle the CanExecute automatically
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error setting SelectedCar: {ex.Message}");
                }
            }
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
        public ICommand SelectCarCommand { get; }

        public CarShopViewModel(IDataService dataService) : base(dataService)
        {
            _cars = new ObservableCollection<Car>();
            LoadCarsCommand = new AsyncRelayCommand(LoadCarsAsync);
            AddToCartCommand = new AsyncRelayCommand(AddToCartAsync, () => SelectedCar != null);
            SelectCarCommand = new Command<Car>(OnSelectCar);

            LoadCarsCommand.Execute(null);
        }

        private void OnSelectCar(Car car)
        {
            System.Diagnostics.Debug.WriteLine($"SelectCar command executed with car: {car?.Make} {car?.Model}");
            SelectedCar = car;
        }

        private async Task LoadCarsAsync()
        {
            if (IsBusy) return;
            try
            {
                IsBusy = true;
                StatusMessage = "Loading cars...";

                var carList = await _dataService.GetAllCarsAsync();
                System.Diagnostics.Debug.WriteLine($"Loaded {carList.Count} cars");

                Cars.Clear();
                foreach (var car in carList)
                {
                    Cars.Add(car);
                }

                StatusMessage = $"Loaded {carList.Count} cars";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading cars: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"Error loading cars: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task AddToCartAsync()
        {
            System.Diagnostics.Debug.WriteLine("AddToCartAsync method started");

            if (IsBusy)
            {
                System.Diagnostics.Debug.WriteLine("AddToCartAsync - IsBusy is true, returning");
                return;
            }

            if (SelectedCar == null)
            {
                System.Diagnostics.Debug.WriteLine("AddToCartAsync - SelectedCar is null, returning");
                StatusMessage = "Please select a car first";
                return;
            }

            if (Quantity <= 0)
            {
                System.Diagnostics.Debug.WriteLine("AddToCartAsync - Quantity <= 0, returning");
                StatusMessage = "Please select a valid quantity";
                return;
            }

            try
            {
                IsBusy = true;
                System.Diagnostics.Debug.WriteLine("AddToCartAsync - Getting first profile");
                var profile = await _dataService.GetFirstProfileAsync();

                if (profile == null)
                {
                    System.Diagnostics.Debug.WriteLine("AddToCartAsync - No profile found");
                    StatusMessage = "Please create a profile first";
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"AddToCartAsync - Profile found with ID: {profile.ProfileId}");
                System.Diagnostics.Debug.WriteLine($"AddToCartAsync - Creating cart item for Car ID: {SelectedCar.CarId}, Quantity: {Quantity}");

                var cartItem = new CartItem
                {
                    ProfileId = profile.ProfileId,
                    CarId = SelectedCar.CarId,
                    Quantity = Quantity
                };

                System.Diagnostics.Debug.WriteLine("AddToCartAsync - Calling AddToCartAsync on data service");
                await _dataService.AddToCartAsync(cartItem);
                System.Diagnostics.Debug.WriteLine("AddToCartAsync - Item added to cart successfully");

                StatusMessage = "Added to cart successfully!";
                await LoadCarsAsync(); // Reload to update stock quantities
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"AddToCartAsync - Error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"AddToCartAsync - Stack trace: {ex.StackTrace}");
                StatusMessage = $"Error adding to cart: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
                System.Diagnostics.Debug.WriteLine("AddToCartAsync - Method completed");
            }
        }
    }
}