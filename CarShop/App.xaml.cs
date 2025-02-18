namespace CarShop;

public partial class App : Application
{
    public App()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("Starting App initialization");

            InitializeComponent();
            System.Diagnostics.Debug.WriteLine("InitializeComponent completed");

            MainPage = new AppShell();
            System.Diagnostics.Debug.WriteLine("AppShell set as MainPage");
        }
        catch (Exception ex)
        {  
            System.Diagnostics.Debug.WriteLine($"Error in App constructor: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
        }
    }
}