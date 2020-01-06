namespace FowCube.Services
{
    using FowCube.Models;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Xamarin.Essentials;

    class CardStore : IDataStore<Card>
    {
        bool IsConnected => Connectivity.NetworkAccess == NetworkAccess.Internet;

        public HttpClient Client { get; }

        public IEnumerable<Card> Items { get; set; }

        public CardStore()
        {
            this.Client = new HttpClient
            {
                BaseAddress = new Uri($"{App.AzureBackendUrl}/app/")
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

            this.Items = new List<Card>();
        }

        /// <summary>
        /// Say Hello World to the user.
        /// </summary>
        /// <returns>Nothing other than the welcome.</returns>
        public async Task HelloWorld()
        {
            if (this.IsConnected)
            {
                await this.Client.GetStringAsync($"hello-world").ContinueWith(responseTask =>
                {
                    Console.WriteLine(responseTask.Result);
                });
            }
        }

        public async Task<bool> AddItemAsync(Card item)
        {
            if(item != null || this.IsConnected)
            {
                var serializedItem = JsonConvert.SerializeObject(item);
                var response = await this.Client.PostAsync($"card", new StringContent(serializedItem, Encoding.UTF8, "application/json"));
                return response.IsSuccessStatusCode;
            }

            return false;
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            if (string.IsNullOrEmpty(id) && !this.IsConnected)
                return false;

            var response = await this.Client.DeleteAsync($"card/{id}");

            return response.IsSuccessStatusCode;
        }

        public async Task<Card> GetItemAsync(string id)
        {
            if (id != null && this.IsConnected)
            {
                var json = await this.Client.GetStringAsync($"card/{id}");
                return await Task.Run(() => JsonConvert.DeserializeObject<Card>(json));
            }

            return null;
        }

        public async Task<IEnumerable<Card>> GetItemsAsync(bool forceRefresh = false)
        {
            if (forceRefresh && this.IsConnected)
            {
                var json = await this.Client.GetStringAsync($"card");
                this.Items = await Task.Run(() => JsonConvert.DeserializeObject<IEnumerable<Card>>(json));
            }

            return this.Items;
        }

        public async Task<bool> UpdateItemAsync(Card item)
        {
            if (item == null || item.Id == null || !this.IsConnected)
                return false;

            var serializedItem = JsonConvert.SerializeObject(item);
            var buffer = Encoding.UTF8.GetBytes(serializedItem);
            var byteContent = new ByteArrayContent(buffer);

            var response = await this.Client.PutAsync(new Uri($"card/{item.Id}"), byteContent);

            return response.IsSuccessStatusCode;
        }
    }
}
