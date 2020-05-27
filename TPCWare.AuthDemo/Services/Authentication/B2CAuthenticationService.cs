using Microsoft.Identity.Client;

using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TPCWare.AuthDemo.Models;

using Xamarin.Essentials;
using Xamarin.Forms;

namespace TPCWare.AuthDemo.Services
{
    /// <summary>
    ///  For simplicity, we'll have this as a singleton. 
    /// </summary>
    public class B2CAuthenticationService : IAuthenticationService
    {
        public event EventHandler AuthenticationStateChanged;

        public UserContext UserContext { get; private set; } = null;

        // TODO: this should be refactored using settings and user secrets

        // Azure AD B2C Coordinates
        public static string Tenant = "bagno38.onmicrosoft.com";
        public static string AzureADB2CHostname = "bagno38.b2clogin.com";
        public static string ClientID = "93141eeb-51fa-43ad-8a10-06c50879ce89";
        public static string PolicySignUpSignIn = "B2C_1_RegisterAndLogin";
        public static string PolicyEditProfile = "";
        public static string PolicyResetPassword = "";

        public static string[] Scopes = { "https://fabrikamb2c.onmicrosoft.com/helloapi/demo.read" };

        public static string AuthorityBase = $"https://{AzureADB2CHostname}/tfp/{Tenant}/";
        public static string AuthoritySignInSignUp = $"{AuthorityBase}{PolicySignUpSignIn}";
        public static string AuthorityEditProfile = $"{AuthorityBase}{PolicyEditProfile}";
        public static string AuthorityPasswordReset = $"{AuthorityBase}{PolicyResetPassword}";
        public static string IOSKeyChainGroup = "com.microsoft.adalcache";

        private readonly IPublicClientApplication _pca;

        public B2CAuthenticationService()
        {
            // default redirectURI; each platform specific project will have to override it with its own
            var builder = PublicClientApplicationBuilder.Create(ClientID)
                .WithB2CAuthority(AuthoritySignInSignUp)
                .WithIosKeychainSecurityGroup(IOSKeyChainGroup)
                .WithRedirectUri($"msal{ClientID}://auth");

            // Android implementation is based on https://github.com/jamesmontemagno/CurrentActivityPlugin
            // iOS implementation would require to expose the current ViewControler - not currently implemented as it is not required
            // UWP does not require this
            var windowLocatorService = DependencyService.Get<IParentWindowLocatorService>();

            if (windowLocatorService != null)
            {
                builder = builder.WithParentActivityOrWindow(() => windowLocatorService?.GetCurrentParentWindow());
            }

            _pca = builder.Build();
        }

        protected virtual void OnAuthenticationStateChanged()
        {
            AuthenticationStateChanged?.Invoke(this, EventArgs.Empty);
        }

        //private void ClearAccessToken()
        //{
        //    SecureStorage.Remove("AccessToken");
        //}

        private async Task SetAccessTokenAsync(string accessToken)
        {
            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                // Store the access token securely for later use.
                await SecureStorage.SetAsync("AccessToken", accessToken);
            }
            else
            {
                SecureStorage.Remove("AccessToken");
            }
        }

        public async Task<bool> SignInAsync()
        {
            bool result = true;
            UserContext userContext = new UserContext();
            try
            {
                // acquire token silent
                userContext = await AcquireTokenSilent();
            }
            catch (MsalUiRequiredException)
            {
                // acquire token interactive
                userContext = await SignInInteractively();
            }
            catch (Exception)
            {
                result = false;
            }
            finally
            {
                await SetAccessTokenAsync(userContext?.AccessToken);
                UserContext = userContext;
                OnAuthenticationStateChanged();
            }
            return result;
        }

        private async Task<UserContext> AcquireTokenSilent()
        {
            IEnumerable<IAccount> accounts = await _pca.GetAccountsAsync();
            AuthenticationResult authResult = await _pca.AcquireTokenSilent(Scopes, GetAccountByPolicy(accounts, PolicySignUpSignIn))
               .WithB2CAuthority(AuthoritySignInSignUp)
               .ExecuteAsync();

            var newContext = UpdateUserInfo(authResult);
            return newContext;
        }

        private async Task<UserContext> SignInInteractively()
        {
            IEnumerable<IAccount> accounts = await _pca.GetAccountsAsync();

            AuthenticationResult authResult = await _pca.AcquireTokenInteractive(Scopes)
                .WithAccount(GetAccountByPolicy(accounts, PolicySignUpSignIn))
                .ExecuteAsync();

            var newContext = UpdateUserInfo(authResult);
            return newContext;
        }

        public async Task<bool> SignOutAsync()
        {
            IEnumerable<IAccount> accounts = await _pca.GetAccountsAsync();
            while (accounts.Any())
            {
                await _pca.RemoveAsync(accounts.FirstOrDefault());
                accounts = await _pca.GetAccountsAsync();
            }
            SecureStorage.Remove("AccessToken");
            UserContext = new UserContext
            {
                IsLoggedOn = false
            };
            OnAuthenticationStateChanged();
            return true;
        }

        public async Task<bool> ResetPasswordAsync()
        {
            AuthenticationResult authResult = await _pca.AcquireTokenInteractive(Scopes)
                .WithPrompt(Prompt.NoPrompt)
                .WithAuthority(AuthorityPasswordReset)
                .ExecuteAsync();

            UserContext = UpdateUserInfo(authResult);
            await SetAccessTokenAsync(UserContext?.AccessToken);
            OnAuthenticationStateChanged();
            return true;
        }

        public async Task<bool> EditProfileAsync()
        {
            IEnumerable<IAccount> accounts = await _pca.GetAccountsAsync();

            AuthenticationResult authResult = await _pca.AcquireTokenInteractive(Scopes)
                .WithAccount(GetAccountByPolicy(accounts, PolicyEditProfile))
                .WithPrompt(Prompt.NoPrompt)
                .WithAuthority(AuthorityEditProfile)
                .ExecuteAsync();

            UserContext = UpdateUserInfo(authResult);
            await SetAccessTokenAsync(UserContext?.AccessToken);
            OnAuthenticationStateChanged();
            return true;
        }

        private IAccount GetAccountByPolicy(IEnumerable<IAccount> accounts, string policy)
        {
            foreach (var account in accounts)
            {
                string userIdentifier = account.HomeAccountId.ObjectId.Split('.')[0];
                if (userIdentifier.EndsWith(policy.ToLower())) return account;
            }

            return null;
        }

        private string Base64UrlDecode(string s)
        {
            s = s.Replace('-', '+').Replace('_', '/');
            s = s.PadRight(s.Length + (4 - s.Length % 4) % 4, '=');
            var byteArray = Convert.FromBase64String(s);
            var decoded = Encoding.UTF8.GetString(byteArray, 0, byteArray.Count());
            return decoded;
        }

        private UserContext UpdateUserInfo(AuthenticationResult ar)
        {
            var newContext = new UserContext();
            newContext.IsLoggedOn = false;
            JObject user = ParseIdToken(ar.IdToken);

            newContext.AccessToken = ar.AccessToken;
            newContext.Name = user["name"]?.ToString();
            newContext.UserIdentifier = user["oid"]?.ToString();

            newContext.GivenName = user["given_name"]?.ToString();
            newContext.FamilyName = user["family_name"]?.ToString();

            newContext.StreetAddress = user["streetAddress"]?.ToString();
            newContext.City = user["city"]?.ToString();
            newContext.Province = user["state"]?.ToString();
            newContext.PostalCode = user["postalCode"]?.ToString();
            newContext.Country = user["country"]?.ToString();

            newContext.JobTitle = user["jobTitle"]?.ToString();

            var emails = user["emails"] as JArray;
            if (emails != null)
            {
                newContext.EmailAddress = emails[0].ToString();
            }
            newContext.IsLoggedOn = true;

            return newContext;
        }

        JObject ParseIdToken(string idToken)
        {
            // Get the piece with actual user info
            idToken = idToken.Split('.')[1];
            idToken = Base64UrlDecode(idToken);
            return JObject.Parse(idToken);
        }
    }
}