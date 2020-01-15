namespace FowCube.Services
{
    using System;
    using System.Net.Http;
    using Xamarin.Essentials;
    using Xamarin.Forms;

    public class BasicStore
    {
        protected bool IsConnected => Connectivity.NetworkAccess == NetworkAccess.Internet;

        public HttpClient Client { get; }

        public BasicStore(string firstEndpoint)
        {
            this.Client = new HttpClient
            {
                BaseAddress = new Uri($"{App.AzureBackendUrl}/{firstEndpoint}/")
            };
            /*try
            {
                this.Client.DefaultRequestHeaders.Add("Accept", "application/json");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }*/
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
