namespace FowCube
{
    using Xamarin.Essentials;
    using Xamarin.Forms;
    using FowCube.Services;
    using FowCube.Views;
    using System.Threading.Tasks;

    public partial class App : Application
    {
        // TODO: Replace with *.azurewebsites.net url after deploying backend to Azure.
        // To debug on Android emulators run the web backend against .NET Core not IIS.
        // If using other emulators besides stock Google images you may need to adjust the IP address.
        // Now there's the url to firebase.
        public static string AzureBackendUrl =
            DeviceInfo.Platform == DevicePlatform.Android ? "https://us-central1-fowcube.cloudfunctions.net/app" : "http://localhost:5000";

        public App()
        {
            this.InitializeComponent();
            DependencyService.Register<CardStore>();
            this.MainPage = new MainPage();
        }

        protected override void OnStart()
        {
            // Handle when your app starts.
            /*var adapter = new RestAdapter("http://jordanthoms.apiary.io/");
            var service = adapter.Create<ICardService>();
            var helloResponse = service.HelloWorld();
            _ = helloResponse.Data;*/
            var store = new CardStore();
            var res = Task.Run(() => store.HelloWorld());
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps.
        }

        protected override void OnResume()
        {
            // Handle when your app resumes.
        }
    }
}
