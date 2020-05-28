using System;
using System.Threading.Tasks;
using MvvmHelpers.Commands;
using TPCWare.AuthDemo.Models;
using TPCWare.AuthDemo.Services;
using Xamarin.Forms;
using System.ComponentModel;


namespace TPCWare.AuthDemo.ViewModels
{
    public class AppShellViewModel : ViewModelBase, INotifyPropertyChanged
    {
        public AppShellViewModel() : base()
        {
            Title = "Auth demo";

            LogInOutTitle = "Login";

            AuthenticationService.AuthenticationStateChanged += OnAuthenticationStateChanged;
        }

        private void OnAuthenticationStateChanged(object sender, EventArgs e)
        {
            LogInOutTitle = !(AuthenticationService.UserContext?.IsLoggedOn) ?? true ? "Login" : "Logout";

            MessagingCenter.Send<AppShellViewModel, string>(this, "changeTitle", LogInOutTitle);

            OnPropertyChanged(nameof(LogInOutTitle));
            OnPropertyChanged(nameof(LogInOutIconName));
        }

        public string LogInOutTitle
        { get; private set; }

        // public string LogInOutTitle => !AuthenticationService.UserContext?.IsLoggedOn ?? true ? "Login" : "Logout";

        public string LogInOutIconName => !AuthenticationService.UserContext?.IsLoggedOn ?? true ? "tab_login.png" : "tab_logout.png";
    }
}
