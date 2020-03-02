namespace FowCube
{
    using Xamarin.Essentials;
    using Xamarin.Forms;
    using FowCube.Views;
    using FowCube.Authentication;
    using System.IO;
    using System;
    using FowCube.Services.Database;

    public partial class App : Application
    {
        // TODO: Replace with *.azurewebsites.net url after deploying backend to Azure.
        // To debug on Android emulators run the web backend against .NET Core not IIS.
        // If using other emulators besides stock Google images you may need to adjust the IP address.
        // Now there's the url to firebase.
        public static string AzureBackendUrl =
            DeviceInfo.Platform == DevicePlatform.Android ? "https://us-central1-fowcube.cloudfunctions.net" : "http://localhost:5000";

        static Database database;
        public static Database Database
        {
            get
            {
                if (database == null)
                {
                    database = new Database(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "collection.db3"));
                }
                return database;
            }
        }

        public App()
        {
            this.InitializeComponent();
            // DependencyService.Register<CardStore>();
            // DependencyService.Register<CollectionStore>();
            var authInfo = DependencyService.Get<IAuth>();

            // Check if user is already logged.
            if(authInfo.GetAuthenticatedUser() != null) {
                this.MainPage = new MainPage();
            } else {
                this.MainPage = new LoginPage();
            }
        }

        protected override void OnStart()
        {
            // Handle when your app starts.
            // TODO: Manage mGoogleSignIn Connection.
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps.
        }

        protected override void OnResume()
        {
            // Handle when your app resumes.
            // TODO: Manage mGoogleSignIn Disconnection.
        }
    }
}
