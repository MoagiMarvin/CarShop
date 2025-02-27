using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CarShop.Data;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using CarShop.Model;

namespace CarShop.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        protected readonly IDataService _dataService;
        private bool _isBusy;

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        public BaseViewModel(IDataService dataService)
        {
            _dataService = dataService;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        // In BaseViewModel.cs
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            try
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in OnPropertyChanged: {ex.Message}");
            }
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}

   