using System;
using System.Threading.Tasks;

using Newtonsoft.Json;

using TPCWare.AuthDemo.Models;
using TPCWare.AuthDemo.Services;

namespace TPCWare.AuthDemo.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        FBAccount _fBAccount;


        public LoginViewModel() : base()
        {
            Title = "Login";

            AuthenticationService.AuthenticationStateChanged += OnAuthenticationStateChanged;
        }

        private void OnAuthenticationStateChanged(object sender, EventArgs e)
        {
            Title = AuthenticationService?.UserContext.IsLoggedOn ?? false ? "Logout" : "Login";

            if (AuthenticationService.UserContext.IsLoggedOn) ReadFacebookData();

            OnPropertyChanged(nameof(Title));
            OnPropertyChanged(nameof(LoggedUserMsg));


        }

        public string LoggedUserMsg => $"User is logged and the Access Token is: {AuthenticationService?.UserContext?.AccessToken ?? "N/A"}";

        public FBAccount FBAccount
        {
            get { return _fBAccount; }
            set
            {
                if (this._fBAccount == value)
                {
                    return;
                }

                this._fBAccount = value;
                this.OnPropertyChanged();
            }
        }

        public void ReadFacebookData()
        {
            var facebookClient = new FacebookClient();
            var facebookService = new FacebookService(facebookClient);
            //var getAccountTask = facebookService.GetAccountAsync(AuthenticationService.UserContext.IdpAccessToken);

            var getAccountTaskResult = Task.Run(() => facebookService.GetAccountAsync(AuthenticationService.UserContext.IdpAccessToken)).Result;
            //Task.WaitAll(getAccountTask);
            //FBAccount = getAccountTask.Result;

            FBAccount = getAccountTaskResult;
            this.OnPropertyChanged();
        }

    }
}
