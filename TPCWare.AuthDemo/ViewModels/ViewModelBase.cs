using System;
using System.Threading.Tasks;

using MvvmHelpers;
using MvvmHelpers.Commands;

using TPCWare.AuthDemo.Models;
using TPCWare.AuthDemo.Services;

using Xamarin.Essentials;
using Xamarin.Forms;

namespace TPCWare.AuthDemo.ViewModels
{
    public class ViewModelBase : BaseViewModel
    {
        protected readonly IDataStore<Item> DataStore;
        protected readonly IAuthenticationService AuthenticationService;

        public AsyncCommand LoginCommand { get; }
        public AsyncCommand LogoutCommand { get; }
        public AsyncCommand AbortCommand { get; }

        public ViewModelBase()
        {
            DataStore = DependencyService.Get<IDataStore<Item>>();
            AuthenticationService = DependencyService.Get<IAuthenticationService>();

            LoginCommand = new AsyncCommand(UserLogin);
            LogoutCommand = new AsyncCommand(UserLogout);
            AbortCommand = new AsyncCommand(UserLoginAbort);

            AuthenticationService.AuthenticationStateChanged += OnAuthenticationStateChanged;
        }

        private void OnAuthenticationStateChanged(object sender, EventArgs e)
        {
            OnPropertyChanged(nameof(IsUserLogged));
            OnPropertyChanged(nameof(IsUserNotLogged));
        }

        bool isLogging;
        public bool IsLogging
        {
            get { return isLogging; }
            private set
            {
                if (SetProperty(ref isLogging, value))
                {
                    OnPropertyChanged(nameof(IsNotLogging));
                }
            }
        }

        public bool IsNotLogging => !IsLogging;

        public bool IsUserLogged => AuthenticationService.UserContext?.IsLoggedOn ?? false;

        public bool IsUserNotLogged => !IsUserLogged;


        async Task UserLogin()
        {
            if (!IsLogging)
            {
                IsLogging = true;
                try
                {
                    // TODO: Analytics.TrackEvent("UserLogin");
                    await AuthenticationService.SignInAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    IsLogging = false;
                }
                finally
                {
                    IsLogging = false;
                }
            }
        }

        async Task UserLogout()
        {
            if (!IsLogging)
            {
                IsLogging = true;
                try
                {
                    // TODO: Analytics.TrackEvent("UserLogin");
                    await AuthenticationService.SignOutAsync();
                }
                catch (Exception)
                {
                }
                finally
                {
                    IsLogging = false;
                }
            }
        }

        async Task UserLoginAbort()
        {
            if (IsLogging)
            {
                try
                {
                    // TODO: Analytics.TrackEvent("UserLoginAbort");
                    // TODO: Task cancellation of AuthService.Signin
                    await Task.Delay(1000);
                }
                catch (Exception)
                {
                }
                finally
                {
                    // TODO: IsLogging = false;
                }
            }
        }

        public async Task<bool> CheckConnectivity(string title, string message)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                return true;

            await DisplayAlert(title, message);
            return false;
        }

        public Task DisplayAlert(string title, string message) =>
            Application.Current.MainPage.DisplayAlert(title, message, "OK");

        public Task<bool> DisplayAlert(string title, string message, string accept, string cancel) =>
            Application.Current.MainPage.DisplayAlert(title, message, accept, cancel);
    }
}
