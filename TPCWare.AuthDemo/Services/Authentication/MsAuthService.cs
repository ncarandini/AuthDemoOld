using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace TPCWare.AuthDemo.Services
{
    public class MsAuthService
    {
        // Azure AD B2C
        public static string Tenant = "bagno38.onmicrosoft.com";
        public static string AzureADB2CHostname = "bagno38.b2clogin.com";
        public static string ClientID = "93141eeb-51fa-43ad-8a10-06c50879ce89";
        public static string PolicySignUpSignIn = "B2C_1_RegisterAndLogin";
        public static string PolicyEditProfile = "";
        public static string PolicyResetPassword = "";

        // Public Client Application 
        public static string AuthorityBase = $"https://{AzureADB2CHostname}/tfp/{Tenant}/";
        public static string Authority = $"{AuthorityBase}{PolicySignUpSignIn}";
        public static string AuthorityEditProfile = $"{AuthorityBase}{PolicyEditProfile}";
        public static string AuthorityPasswordReset = $"{AuthorityBase}{PolicyResetPassword}";

        // Mobile MSAL
        readonly string AppId = "com.tpcware.authdemo";
        readonly string AndroidSignatureHash = "AyU0djuOW6JHov33Q0hsa8UDqk4=";
        readonly string[] Scopes = { "Read" };

        readonly IPublicClientApplication _pca;

        string RedirectUri
        {
            get
            {
                if (DeviceInfo.Platform == DevicePlatform.Android)
                    return $"msauth://{AppId}/{{{AndroidSignatureHash}}}";
                else if (DeviceInfo.Platform == DevicePlatform.iOS)
                    return $"msauth.{AppId}://auth";

                return string.Empty;
            }
        }

        public string AccessToken { get; private set; } = null;

        // Android uses this to determine which activity to use to show
        // the login screen dialog from.
        public static object ParentWindow { get; set; }

        public MsAuthService()
        {
            _pca = PublicClientApplicationBuilder.Create(ClientID)               .WithB2CAuthority(Authority)               .Build();

            //_pca = PublicClientApplicationBuilder.Create(ClientID)
            //        .WithIosKeychainSecurityGroup(AppId)
            //        .WithRedirectUri(RedirectUri)
            //        .WithAuthority("https://login.microsoftonline.com/common")
            //        .Build();

            // TODO: check the right way to call async method on constructor method
            // and see if we need set it a public method, to call it from outside.
            Task.Run(() => this.InitializeAsync()).Wait();
        }

        private async Task InitializeAsync()
        {
            AccessToken = await SecureStorage.GetAsync("AccessToken");
        }

        private void ClearAccessToken()
        {
            AccessToken = null;
            SecureStorage.Remove("AccessToken");
        }

        public async Task<bool> SignInAsync()
        {
            bool result = false;
            try
            {
                var accounts = await _pca.GetAccountsAsync();
                var firstAccount = accounts.FirstOrDefault();
                var authResult = await _pca.AcquireTokenSilent(Scopes, firstAccount).ExecuteAsync();

                // Store the access token securely for later use.
                await SecureStorage.SetAsync("AccessToken", authResult?.AccessToken);

                return true;
            }
            catch (MsalUiRequiredException)
            {
                try
                {
                    // This means we need to login again through the MSAL window.
                    var authResult = await _pca.AcquireTokenInteractive(Scopes)
                                                .WithParentActivityOrWindow(ParentWindow)
                                                .WithUseEmbeddedWebView(true)
                                                .ExecuteAsync();

                    // Store the access token securely for later use.
                    await SecureStorage.SetAsync("AccessToken", authResult?.AccessToken);

                    result = true;
                }
                catch (Exception ex2)
                {
                    Debug.WriteLine(ex2.ToString());
                    ClearAccessToken();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                ClearAccessToken();
            }

            return result;
        }

        public async Task<bool> SignOutAsync()
        {
            try
            {
                var accounts = await _pca.GetAccountsAsync();

                // Go through all accounts and remove them.
                while (accounts.Any())
                {
                    await _pca.RemoveAsync(accounts.FirstOrDefault());
                    accounts = await _pca.GetAccountsAsync();
                }

                // Clear our access token from secure storage.
                ClearAccessToken();

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());

                // An error occurred, better clear the access token
                ClearAccessToken();

                return false;
            }
        }
    }
}