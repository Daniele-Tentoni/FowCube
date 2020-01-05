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

    public class CollectionStore
    {
        bool IsConnected => Connectivity.NetworkAccess == NetworkAccess.Internet;

        public HttpClient Client { get; }

        public IEnumerable<Collection> Items { get; set; }

        public CollectionStore()
        {
            this.Client = new HttpClient
            {
                BaseAddress = new Uri($"{App.AzureBackendUrl}/collection/")
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

        private class CreateInfo
        {
            public string Name { get; set; }
            public string Uid { get; set; }
        }

        public async Task<bool> CreateAsync(string name, string uid)
        {
            if (name != null || uid != null || this.IsConnected)
            {
                var serializedItem = JsonConvert.SerializeObject(new CreateInfo {Name = name, Uid = uid});
                var response = await this.Client.PostAsync($"api/collection", new StringContent(serializedItem, Encoding.UTF8, "application/json"));
                return response.IsSuccessStatusCode;
            }

            return false;
        }
        public Task<bool> DeleteItemAsync(string id) => throw new System.NotImplementedException();

        public async Task<Collection> GetAsync(string id, string uid)
        {
            if (id != null && uid != null && this.IsConnected)
            {
                var json = await this.Client.GetStringAsync($"api/collection/{id}/{uid}");
                return await Task.Run(() => JsonConvert.DeserializeObject<Collection>(json));
            }

            return null;
        }

        public Task<IEnumerable<Collection>> GetItemsAsync(bool forceRefresh = false) => throw new System.NotImplementedException();
        public Task<bool> UpdateItemAsync(Collection item) => throw new System.NotImplementedException();
    }
}
