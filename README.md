# AuthDemo
Xamarin Forms demo app to authenticate on Azure AD B2C service using MSAL

## Description
This is a lab project to experiment with AAD B2C Authentication in Xamarin Forms with MSAL, to:

- Use the same User secrets mechanism of .NET Core apps to prevent pushing sensible data to GitHub 
- Store the access token with Xamarin Essentials SecureStorage
- Define an IAuthenticationService interface for the AuthenticationService so we can use Xamarin Forms Dependency Service
- Use a minimalist MVVM approach with James Montemagno MvvmHelpers
- Experiment with different architectural solution
- Experiment with shell UI to integrate the login/logout process and visual representation of User info and status


