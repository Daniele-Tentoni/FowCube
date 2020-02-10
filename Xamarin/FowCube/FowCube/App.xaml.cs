namespace FowCube
{
    using Xamarin.Essentials;
    using Xamarin.Forms;
    using FowCube.Services;
    using FowCube.Views;
    using FowCube.Authentication;
    using FowCube.Services.Collections;
    using Realms;
    using FowCube.Models.Collection;
    using System.Linq;

    public partial class App : Application
    {
        // TODO: Replace with *.azurewebsites.net url after deploying backend to Azure.
        // To debug on Android emulators run the web backend against .NET Core not IIS.
        // If using other emulators besides stock Google images you may need to adjust the IP address.
        // Now there's the url to firebase.
        public static string AzureBackendUrl =
            DeviceInfo.Platform == DevicePlatform.Android ? "https://us-central1-fowcube.cloudfunctions.net" : "http://localhost:5000";

        /// <summary>
        /// Use the migration callback to migrate saftly database from a version to another.
        /// </summary>
        public static RealmConfiguration RealmConfig = new RealmConfiguration()
        {
            SchemaVersion = 2,
            MigrationCallback = (migration, oldSchemaVersion) =>
            {
                var newColls = migration.NewRealm.All<Collection>();
                var oldColls = migration.OldRealm.All<Collection>();

                for (var i = 0; i < newColls.Count(); i++)
                {
                    var oldColl = oldColls.ElementAt(i);
                    var newColl = newColls.ElementAt(i);

                    // Migrate Collection from version 1 to 2: generate the FirebaseId property.
                    if (oldSchemaVersion < 2)
                    {
                        newColl.FirebaseId = oldColl.Id;
                        newColl.Id = oldColl.Id;
                    }
                };

            }
        };

        public App()
        {
            this.InitializeComponent();
            DependencyService.Register<CardStore>();
            DependencyService.Register<CollectionStore>();
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
