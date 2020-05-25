using System;

namespace TPCWare.AuthDemo.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        public LoginViewModel(): base()
        {
            Title = "Login";

            AuthenticationService.AuthenticationStateChanged += OnAuthenticationStateChanged;
        }

        private void OnAuthenticationStateChanged(object sender, EventArgs e)
        {
            Title = AuthenticationService?.UserContext.IsLoggedOn ?? false ? "Logout" : "Login";
            OnPropertyChanged(nameof(Title));
            OnPropertyChanged(nameof(LoggedUserMsg));
        }

        public string LoggedUserMsg => $"User is logged and the Access Token is: {AuthenticationService?.UserContext?.AccessToken ?? "N/A"}";
    }
}
