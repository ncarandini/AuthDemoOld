using System;
using System.Net.Http;
using Microsoft.Identity.Client;
using TPCWare.AuthDemo.Models;
using TPCWare.AuthDemo.Services;
using Xamarin.Forms;

namespace TPCWare.AuthDemo.Views
{
    public partial class MainPage : ContentPage
    {
        public IAuthenticationService AuthenticationService { get; }

        // TODO use viewmodel

        public MainPage()
        {
            InitializeComponent();

            AuthenticationService = DependencyService.Get<IAuthenticationService>();
        }

        async void OnSignInSignOut(object sender, EventArgs e)
        {
            try
            {
                if (btnSignInSignOut.Text == "Sign in")
                {
                    if (await AuthenticationService.SignInAsync())
                    {
                        UpdateSignInState(AuthenticationService.UserContext);
                        UpdateUserInfo(AuthenticationService.UserContext);
                    }
                }
                else
                {
                    if(await AuthenticationService.SignOutAsync())
                    {
                        UpdateSignInState(AuthenticationService.UserContext);
                        UpdateUserInfo(AuthenticationService.UserContext);
                    }
                }
            }
            catch (Exception ex)
            {
                // Checking the exception message 
                // should ONLY be done for B2C
                // reset and not any other error.
                if (ex.Message.Contains("AADB2C90118"))
                    OnPasswordReset();
                // Alert if any exception excluding user canceling sign-in dialog
                else if (((ex as MsalException)?.ErrorCode != "authentication_canceled"))
                    await DisplayAlert($"Exception:", ex.ToString(), "Dismiss");
            }
        }
        async void OnCallApi(object sender, EventArgs e)
        {
            try
            {
                lblApi.Text = $"Calling API {App.ApiEndpoint}";
                await AuthenticationService.SignInAsync();
                var token = AuthenticationService.UserContext.AccessToken;

                // Get data from API
                HttpClient client = new HttpClient();
                HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, App.ApiEndpoint);
                message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await client.SendAsync(message);
                string responseString = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    lblApi.Text = $"Response from API {App.ApiEndpoint} | {responseString}";
                }
                else
                 {
                    lblApi.Text = $"Error calling API {App.ApiEndpoint} | {responseString}";
                }
            }
            catch (MsalUiRequiredException ex)
            {
                await DisplayAlert($"Session has expired, please sign out and back in.", ex.ToString(), "Dismiss");
            }
            catch (Exception ex)
            {
                await DisplayAlert($"Exception:", ex.ToString(), "Dismiss");
            }
        }

        async void OnEditProfile(object sender, EventArgs e)
        {
            try
            {
                await AuthenticationService.EditProfileAsync();
                UpdateSignInState(AuthenticationService.UserContext);
                UpdateUserInfo(AuthenticationService.UserContext);
            }
            catch (Exception ex)
            {
                // Alert if any exception excluding user canceling sign-in dialog
                if (((ex as MsalException)?.ErrorCode != "authentication_canceled"))
                    await DisplayAlert($"Exception:", ex.ToString(), "Dismiss");
            }
        }
        async void OnResetPassword(object sender, EventArgs e)
        {
            try
            {
                await AuthenticationService.ResetPasswordAsync();
                UpdateSignInState(AuthenticationService.UserContext);
                UpdateUserInfo(AuthenticationService.UserContext);
            }
            catch (Exception ex)
            {
                // Alert if any exception excluding user canceling sign-in dialog
                if (((ex as MsalException)?.ErrorCode != "authentication_canceled"))
                    await DisplayAlert($"Exception:", ex.ToString(), "Dismiss");
            }
        }
        async void OnPasswordReset()
        {
            try
            {
                await AuthenticationService.ResetPasswordAsync();
                UpdateSignInState(AuthenticationService.UserContext);
                UpdateUserInfo(AuthenticationService.UserContext);
            }
            catch (Exception ex)
            {
                // Alert if any exception excluding user canceling sign-in dialog
                if (((ex as MsalException)?.ErrorCode != "authentication_canceled"))
                    await DisplayAlert($"Exception:", ex.ToString(), "Dismiss");
            }
        }

        void UpdateSignInState(UserContext userContext)
        {
            var isSignedIn = userContext.IsLoggedOn;
            btnSignInSignOut.Text = isSignedIn ? "Sign out" : "Sign in";
            btnEditProfile.IsVisible = isSignedIn;
            btnCallApi.IsVisible = isSignedIn;
            slUser.IsVisible = isSignedIn;
            lblApi.Text = "";
        }
        public void UpdateUserInfo(UserContext userContext)
        {
            lblName.Text = userContext.Name;
            lblJob.Text = userContext.JobTitle;
            lblCity.Text = userContext.City;
        }
    }
}