namespace FowCube.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using FowCube.Models.Cards;
    using FowCube.Models.Collection;
    using Newtonsoft.Json;

    public class CollectionStore : BasicStore
    {
        public CollectionStore() : base("func_coll") { }

        public class CollectionsInfo
        {
            [JsonProperty(PropertyName = "uid")]
            public string UserId { get; set; }

            [JsonProperty(PropertyName = "collections")]
            public List<BasicCollection> Collections { get; set; }
        }

        /// <summary>
        /// Return the list of all collections of a user.
        /// </summary>
        /// <param name="uid">User Identifier.</param>
        /// <returns>List of collections.</returns>
        public async Task<List<BasicCollection>> GetAllUserCollectionsAsync(string uid)
        {
            if (uid == null) throw new ArgumentNullException();
            if (!this.IsConnected) throw new NotConnectedException();

            var message = await this.Client.GetAsync($"collections/{uid}");
            if(!message.IsSuccessStatusCode)
            {
                if (message.StatusCode.Equals(HttpStatusCode.NotFound)) throw new HttpRequestException(message.ReasonPhrase);
            }

            var result = JsonConvert.DeserializeObject<CollectionsInfo>(message.Content.ReadAsStringAsync().Result);
            return result.Collections;
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
        public async Task<Collection> GetAsync(string collectionId)
        {
            if (collectionId == null) throw new ArgumentNullException();
            if (!this.IsConnected) throw new NotConnectedException();

            var response = await this.Client.GetAsync($"collection/{collectionId}");
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode.Equals(HttpStatusCode.NotFound)) throw new HttpRequestException(response.ReasonPhrase);
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

        /// <summary>
        /// Add a card to the colletion.
        /// </summary>
        /// <param name="collId">Collection Id.</param>
        /// <param name="card">Card Entity.</param>
        /// <returns>True for a successful operation.</returns>
        public async Task<bool> AddCardToCollection(string collId, Card card)
        {
            if (collId == null || card == null) throw new ArgumentNullException();

            var serializedItem = JsonConvert.SerializeObject(card);
            var response = await this.Client.PutAsync($"collection/addcard/{collId}", new StringContent(serializedItem, Encoding.UTF8, "application/json"));
            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Remove a card from a collection.
        /// </summary>
        /// <param name="collId">Collection Id.</param>
        /// <param name="card">Card Entity.</param>
        /// <returns>True for a successful operation.</returns>
        public async Task<bool> RemoveCardFromCollection(string collId, Card card)
        {
            if (collId == null || card == null) throw new ArgumentNullException();

            var serializedItem = JsonConvert.SerializeObject(card);
            var response = await this.Client.PutAsync($"collection/removecard/{collId}", new StringContent(serializedItem, Encoding.UTF8, "application/json"));
            return response.IsSuccessStatusCode;
        }

        public Task<bool> UpdateItemAsync(Collection item) => throw new System.NotImplementedException();
    }
}
