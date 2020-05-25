using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using Xamarin.Forms;
using Xamarin.Essentials;
using TPCWare.AuthDemo.Models;

namespace TPCWare.AuthDemo.Services
{
    public class MockAuthenticationService : IAuthenticationService
    {
        public event EventHandler AuthenticationStateChanged;

        public UserContext UserContext { get; private set; } = null;

        public MockAuthenticationService()
        {
            // TODO: check the right way to call async method on constructor method
            // and see if we need set it a public method, to call it from outside.
            // Task.Run(() => this.InitializeAsync()).Wait();
        }

        //private async Task InitializeAsync()
        //{
        //    // TODO: Get UserContext data from settings but AccessToken,
        //    //       that has to be retrieved from secure storage

        //    AccessToken = await SecureStorage.GetAsync("AccessToken");
        //}

        protected virtual void OnAuthenticationStateChanged()
        {
            AuthenticationStateChanged?.Invoke(this, EventArgs.Empty);
        }

        private void ClearAccessToken()
        {
            SecureStorage.Remove("AccessToken");
        }

        public async Task<bool> SignInAsync()
        {
            await Task.Delay(1000);

            UserContext = new UserContext
            {
                IsLoggedOn = true,
                AccessToken = "FakeAccessToken",
                UserIdentifier = Guid.NewGuid().ToString(),
                GivenName = "John",
                FamilyName = "Smith",
                Name = "John Smith",
                EmailAddress = "john.smith@fakemail.com"
            };

            // Store the access token securely for later use.
            await SecureStorage.SetAsync("AccessToken", UserContext.AccessToken);

            OnAuthenticationStateChanged();

            return true;
        }

        public async Task<bool> SignOutAsync()
        {
            await Task.Delay(1000);

            UserContext = new UserContext
            {
                IsLoggedOn = false,
                AccessToken = string.Empty
            };

            ClearAccessToken();

            OnAuthenticationStateChanged();

            return true;
        }

        public async Task<bool> ResetPasswordAsync()
        {
            // Nothing to do
            await Task.Delay(1000);

            OnAuthenticationStateChanged();

            return true;
        }

        public async Task<bool> EditProfileAsync()
        {
            // Nothing to do
            await Task.Delay(1000);

            OnAuthenticationStateChanged();

            return true;
        }
    }
}
