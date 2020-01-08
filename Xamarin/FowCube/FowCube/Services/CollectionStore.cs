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

        public CollectionStore()
        {
            this.Client = new HttpClient
            {
                BaseAddress = new Uri($"{App.AzureBackendUrl}/app/")
            };

            try
            {
                this.Client.DefaultRequestHeaders.Add("Accept", "application/json");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private class CreateInfo
        {
            [JsonProperty(PropertyName = "name")]
            public string Name { get; set; }
            [JsonProperty(PropertyName = "uid")]
            public string Uid { get; set; }
        }

        /// <summary>
        /// Create a new collection by the user.
        /// </summary>
        /// <param name="name">The name of the collection.</param>
        /// <param name="uid">The creator id.</param>
        /// <returns>Return the id of the collection created.</returns>
        public async Task<string> CreateAsync(string name, string uid)
        {
            if (name == null || uid == null || !this.IsConnected) return null;

            var serializedItem = JsonConvert.SerializeObject(new CreateInfo { Name = name, Uid = uid });
            var response = await this.Client.PostAsync($"collection", new StringContent(serializedItem, Encoding.UTF8, "application/json"));
            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsStringAsync().Result;
            }

            return null;
        }

        public Task<bool> DeleteItemAsync(string id) => throw new System.NotImplementedException();

        /// <summary>
        /// Get a collection by id and userid.
        /// TODO: Develop the multi-collection feature.
        /// </summary>
        /// <param name="id">Id of the collection.</param>
        /// <param name="uid">Id of the user.</param>
        /// <returns>Return the selected collection.</returns>
        public async Task<Collection> GetAsync(string id, string uid)
        {
            if (id == null || uid == null || !this.IsConnected) return null;

            var response = await this.Client.GetAsync($"collection/{uid}/{id}");
            if (!response.IsSuccessStatusCode)
            {
                // throw new HttpRequestException(response.StatusCode.ToString(), new HttpRequestException(response.Content.ReadAsStringAsync().Result));
                return null;
            }

            return await Task.Run(() => JsonConvert.DeserializeObject<Collection>(response.Content.ReadAsStringAsync().Result));
        }

        /// <summary>
        /// Add a card to the collection.
        /// TODO: Refactor this operation to work better.
        /// </summary>
        /// <param name="collId">Collection Id.</param>
        /// <param name="cardsIn">Cards array list.</param>
        /// <returns>True for a successful operation.</returns>
        public async Task<bool> AddCardToCollection(string collId, string[] cardsIn)
        {
            if(collId != null && cardsIn != null && cardsIn.Length > 0 && this.IsConnected)
            {
                var serializedItem = JsonConvert.SerializeObject(cardsIn);
                var response = await this.Client.PutAsync($"collection/addcard/{collId}", new StringContent(serializedItem, Encoding.UTF8, "application/json"));
                return response.IsSuccessStatusCode;
            }

            return false;
        }

        public async Task<bool> AddCardToCollection(string collId, Card card)
        {
            if (collId == null || card == null) {
                throw new ArgumentNullException();
            }

            var serializedItem = JsonConvert.SerializeObject(card);
            var response = await this.Client.PutAsync($"collection/addcard/{collId}", new StringContent(serializedItem, Encoding.UTF8, "application/json"));
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> RemoveCardFromCollection(string collId, string[] cardsOut)
        {
            if (collId != null && cardsOut != null && cardsOut.Length > 0 && this.IsConnected)
            {
                var serializedItem = JsonConvert.SerializeObject(cardsOut);
                var response = await this.Client.PutAsync($"collection/removecard/{collId}", new StringContent(serializedItem, Encoding.UTF8, "application/json"));
                return response.IsSuccessStatusCode;
            }

            return false;
        }

        public Task<bool> UpdateItemAsync(Collection item) => throw new System.NotImplementedException();
    }
}
