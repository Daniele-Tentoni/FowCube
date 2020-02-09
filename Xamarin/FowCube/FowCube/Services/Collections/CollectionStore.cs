namespace FowCube.Services.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using FowCube.Models.Cards;
    using FowCube.Models.Collection;
    using Newtonsoft.Json;
    using Xamarin.Forms.Internals;

    /// <summary>
    /// Represent the CollectionStore core.
    /// </summary>
    public partial class CollectionStore : BasicStore
    {
        private readonly string TAG = "COLLECTION STORE";

        public CollectionStore() : base("func_coll") { }

        public class CollectionsInfo
        {
            [JsonProperty(PropertyName = "uid")]
            public string UserId { get; set; }

            [JsonProperty(PropertyName = "collections")]
            public List<Collection> Collections { get; set; }
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
        public async Task<string> CreateCollectionAsync(string name, string uid)
        {
            if (name == null || uid == null) return null;

            // TODO: This id is not correct.
            string collId = string.Empty;
            if (this.IsConnected)
            {
                var serializedItem = JsonConvert.SerializeObject(
                    new CreateInfo { Name = name, Uid = uid }
                    );
                var response = await this.Client.PostAsync($"collection",
                    new StringContent(serializedItem, Encoding.UTF8, "application/json")
                    );
                if (response.IsSuccessStatusCode)
                    collId = await response.Content.ReadAsStringAsync();
            }

            var coll = new Collection { Id = collId, Name = name, Uid = uid };
            this.Realm.Write(() => this.Realm.Add(coll));
            coll = this.Realm.All<Collection>().SingleOrDefault(s => s.Id == coll.Id);
            return coll.Id;
        }

        /// <summary>
        /// Return the list of all collections of a user.
        /// Use this at startup to generate all menu voices.
        /// </summary>
        /// <param name="uid">User Identifier.</param>
        /// <returns>List of collections.</returns>
        public async Task<List<Collection>> GetAllUserCollectionsAsync(string uid, bool forceUpdate = false)
        {
            if (uid == null) throw new ArgumentNullException();
            if (!this.IsConnected) throw new NotConnectedException();

            if (forceUpdate)
            {
                var message = await this.Client.GetAsync($"collections/{uid}");
                if (!message.IsSuccessStatusCode)
                {
                    if (message.StatusCode.Equals(HttpStatusCode.NotFound))
                        throw new HttpRequestException(message.ReasonPhrase);
                }

                // Convert the result and save it in Realm.
                var result = JsonConvert.DeserializeObject<CollectionsInfo>(message.Content.ReadAsStringAsync().Result);
                using (var tran = this.Realm.BeginWrite())
                {
                    try
                    {
                        result.Collections.ForEach(collection => this.Realm.Add(collection, true));
                        tran.Commit();
                    }
                    catch (Exception e)
                    {
                        Log.Warning(this.TAG, $"Exception thrown while adding collections to realms.\n{e.Message}");
                        tran.Rollback();
                        throw e;
                    }
                }
            }

            return this.Realm.All<Collection>().Where(w => w.Uid == uid).ToList();
        }

        public Task<bool> DeleteItemAsync(string id) => throw new NotImplementedException();

        /// <summary>
        /// Get a collection by id and userid.
        /// TODO: Develop the multi-collection feature.
        /// </summary>
        /// <param name="id">Id of the collection.</param>
        /// <param name="uid">Id of the user.</param>
        /// <returns>Return the selected collection.</returns>
        public async Task<Collection> GetAsync(string collectionId, bool forceUpdate = false)
        {
            if (collectionId == null) throw new ArgumentNullException();
            if (!this.IsConnected) throw new NotConnectedException();

            // Check if a collection is in Realm.
            var collection = this.Realm.Find<Collection>(collectionId);
            if (collection == null || forceUpdate)
            {
                var response = await this.Client.GetAsync($"collection/{collectionId}");
                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode.Equals(HttpStatusCode.NotFound))
                        throw new HttpRequestException(response.ReasonPhrase);
                    return null;
                }
                collection = await Task.Run(() => JsonConvert.DeserializeObject<Collection>(response.Content.ReadAsStringAsync().Result));
                using (var tran = this.Realm.BeginWrite())
                {
                    this.Realm.Add(collection, true);
                    tran.Commit();
                }
            }

            return collection;
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
            if (collId != null && cardsIn != null && cardsIn.Length > 0 && this.IsConnected)
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
            var response = await this.Client.PutAsync($"collection/addcard/{collId}",
                new StringContent(serializedItem, Encoding.UTF8, "application/json")
                );
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
            var response = await this.Client.PutAsync($"collection/removecard/{collId}",
                new StringContent(serializedItem, Encoding.UTF8, "application/json")
                );
            return response.IsSuccessStatusCode;
        }
    }
}
