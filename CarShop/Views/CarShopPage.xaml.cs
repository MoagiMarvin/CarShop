using CarShop.Model;
using CarShop.ViewModels;

namespace CarShop.Views;

public partial class CarShopPage : ContentPage
{
    private readonly CarShopViewModel _viewModel;

   

    public CarShopPage(CarShopViewModel viewModel)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("Starting CarShopPage initialization");
            InitializeComponent();
            System.Diagnostics.Debug.WriteLine("InitializeComponent completed");
            _viewModel = viewModel;
            BindingContext = _viewModel;
            System.Diagnostics.Debug.WriteLine("ViewModel set as BindingContext");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in CarShopPage constructor: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
        }

    }
    private void OnAddToCartClicked(object sender, EventArgs e)
    {
        System.Diagnostics.Debug.WriteLine("Add to cart button clicked");
    }
    private void OnCarSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Car selectedCar)
        {
            System.Diagnostics.Debug.WriteLine($"Car selected: {selectedCar.Make} {selectedCar.Model}");
            // This forces the binding to update
            ((CarShopViewModel)BindingContext).SelectedCar = selectedCar;
        }
    } 

    
}

