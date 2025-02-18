using CarShop.Views;

namespace CarShop;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Add debug line to see if we get here
        System.Diagnostics.Debug.WriteLine("AppShell initialized");
    }
}