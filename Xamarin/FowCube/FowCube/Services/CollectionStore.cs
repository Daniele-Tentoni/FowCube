namespace FowCube.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using FowCube.Models;
    using Newtonsoft.Json;
    using Xamarin.Essentials;

    class CollectionStore : IDataStore<Collection>
    {
        bool IsConnected => Connectivity.NetworkAccess == NetworkAccess.Internet;

        public HttpClient Client { get; }

        public IEnumerable<Collection> Items { get; set; }

        public CollectionStore()
        {
            this.Client = new HttpClient
            {
                BaseAddress = new Uri($"https://us-central1-fowcube.cloudfunctions.net/cards/")
            };
            try
            {
                this.Client.DefaultRequestHeaders.Add("Accept", "application/json");
                // this.Client.DefaultRequestHeaders.Add("Content-Type", "application/json");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            this.Items = new List<Collection>();
        }

        public async Task<bool> AddItemAsync(Collection item)
        {
            if (item != null || this.IsConnected)
            {
                var serializedItem = JsonConvert.SerializeObject(item);
                var response = await this.Client.PostAsync($"api/card", new StringContent(serializedItem, Encoding.UTF8, "application/json"));
                return response.IsSuccessStatusCode;
            }

            return false;
        }

        public Task<bool> DeleteItemAsync(string id) => throw new System.NotImplementedException();
        public Task<Collection> GetItemAsync(string id) => throw new System.NotImplementedException();
        public Task<IEnumerable<Collection>> GetItemsAsync(bool forceRefresh = false) => throw new System.NotImplementedException();
        public Task<bool> UpdateItemAsync(Collection item) => throw new System.NotImplementedException();
    }
}
