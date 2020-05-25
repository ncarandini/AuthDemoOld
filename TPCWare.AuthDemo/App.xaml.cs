using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using TPCWare.AuthDemo.Services;
using TPCWare.AuthDemo.Views;

namespace TPCWare.AuthDemo
{
    public partial class App : Application
    {
        //TODO: Replace with *.azurewebsites.net url after deploying backend to Azure
        //To debug on Android emulators run the web backend against .NET Core not IIS
        //If using other emulators besides stock Google images you may need to adjust the IP address
        public static string AzureBackendUrl =
            DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:5000" : "http://localhost:5000";

        // TODO: get rid of this!
        public static string ApiEndpoint = "https://fabrikamb2chello.azurewebsites.net/hello";

        public static bool UseMockAuthentication = false;
        public static bool UseMockDataStore = true;

        public App()
        {
            InitializeComponent();

            // Register the Auth service
            if (UseMockAuthentication)
            {
                DependencyService.Register<MockAuthenticationService>();
            }
            else
            {
                DependencyService.Register<B2CAuthenticationService>();
            }

            // Register the DataStore service
            if (UseMockDataStore)
            {
                DependencyService.Register<MockDataStore>();
            }
            else
            {
               DependencyService.Register<AzureDataStore>();
            }

            /* NOTE on Dependency Injection in Xamarin:
             * 
             * 'B2CAuthenticationService' implements the 'IAuthenticationService' interface. 
             * Using the DependencyService we can register the 'B2CAuthenticationService' such 
             * that when we ask for an instance of the 'IAuthenticationService' like this:
             * 
             *      var authenticationService = DependencyService.Get<IAuthenticationService>();
             * 
             * it allows us to grab the instance of the B2CAuthenticationService that we register in the line below:
             * 
             * */
            DependencyService.Register<B2CAuthenticationService>();

            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
