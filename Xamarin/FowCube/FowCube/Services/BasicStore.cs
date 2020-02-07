namespace FowCube.Services
{
    using Realms;
    using System;
    using System.Net.Http;
    using Xamarin.Essentials;
    using Xamarin.Forms;

    public class BasicStore
    {
        protected bool IsConnected => Connectivity.NetworkAccess == NetworkAccess.Internet;
        public HttpClient Client { get; }
        public Realm Realm { get; }

        public BasicStore(string firstEndpoint)
        {
            this.Client = new HttpClient
            {
                BaseAddress = new Uri($"{App.AzureBackendUrl}/{firstEndpoint}/")
            };

            // Get the realm instance.
            this.Realm = Realm.GetInstance();
            /*
             * var authData = string.Format ("{0}:{1}", Constants.Username, Constants.Password);
    var authHeaderValue = Convert.ToBase64String (Encoding.UTF8.GetBytes (authData));

    _client = new HttpClient ();
    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue ("Basic", authHeaderValue);
    */
        }

        /// <summary>
        /// Check any connectivity issue.
        /// May thrown a NotConnectedException.
        /// </summary>
        /// <returns></returns>
        public bool CheckConnectivityIssues()
        {
            if (!this.IsConnected)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Application.Current.MainPage.DisplayAlert("Alert", "You are not connected.", "OK");
                });
                throw new NotConnectedException();
            }
            return true;
        }
    }
}
