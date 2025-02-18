
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using CarShop.Model;
using CarShop.Data;

namespace CarShop.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        private Profile _profile;
        private string _statusMessage;

        public Profile Profile
        {
            get => _profile;
            set => SetProperty(ref _profile, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public ICommand LoadProfileCommand { get; }
        public ICommand SaveProfileCommand { get; }

        public ProfileViewModel(IDataService dataService) : base(dataService)
        {
            _profile = new Profile();
            LoadProfileCommand = new AsyncRelayCommand(LoadProfileAsync);
            SaveProfileCommand = new AsyncRelayCommand(SaveProfileAsync);
            LoadProfileCommand.Execute(null);
        }

        private async Task LoadProfileAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                Profile = await _dataService.GetFirstProfileAsync() ?? new Profile();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading profile: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task SaveProfileAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                await _dataService.SaveProfileAsync(Profile);
                StatusMessage = "Profile saved successfully!";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error saving profile: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}