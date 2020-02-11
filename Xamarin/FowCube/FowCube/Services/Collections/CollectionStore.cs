namespace FowCube.Services.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using FowCube.Models;
    using FowCube.Models.Cards;
    using FowCube.Models.Collection;
    using FowCube.Utils.Strings;
    using Newtonsoft.Json;
    using Xamarin.Forms.Internals;

    /// <summary>
    /// Represent the CollectionStore core.
    /// </summary>
    public partial class CollectionStore : BasicStore
    {
        private readonly string TAG = "COLLECTION STORE";

        public CollectionStore() : base("func_coll") { }

        /// <summary>
        /// Create a new collection by the user.
        /// </summary>
        /// <param name="name">The name of the collection.</param>
        /// <param name="uid">The creator id.</param>
        /// <returns>Return the id of the collection created.</returns>
        public async Task<string> CreateCollectionAsync(string name, string uid, bool forceUpdate = false)
        {
            if (name == null || uid == null) return null;

            // TODO: This id is not correct.
            string collId = string.Empty;
            if (this.IsConnected && forceUpdate)
            {
                var serializedItem = JsonConvert.SerializeObject(new { name, uid });
                var response = await this.Client.PostAsync($"collection",
                    new StringContent(serializedItem, Encoding.UTF8, "application/json")
                    );
                if (response.IsSuccessStatusCode)
                    collId = await response.Content.ReadAsStringAsync();
            }

            try
            {
                await App.Database.CreateCollectionAsync(new Collection { Name = name, User = uid });
            }
            catch (Exception e)
            {
                Debug.WriteLine(string.Format(AppStrings.ExceptionMessage, e.Message));
            }

            return collId;
        }

        public class CollectionsInfo
        {
            [JsonProperty(PropertyName = "uid")]
            public string UserId { get; set; }

            [JsonProperty(PropertyName = "collections")]
            public List<Collection> Collections { get; set; }
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

            if (forceUpdate)
            {
                if (!this.IsConnected) throw new NotConnectedException();

                var message = await this.Client.GetAsync($"collections/{uid}");
                if (message.StatusCode.Equals(HttpStatusCode.NotFound))
                    throw new HttpRequestException(message.ReasonPhrase);

                // Convert the result and save it in Realm.
                var result = JsonConvert.DeserializeObject<List<Collection>>(message.Content.ReadAsStringAsync().Result);
                try
                {
                    result.ForEach(async coll => await App.Database.CreateCollectionAsync(coll));
                }
                catch (Exception e)
                {
                    Log.Warning(this.TAG, $"Exception thrown while adding collections to realms.\n{e.Message}");
                    throw e;
                }
            }

            return await App.Database.GetCollectionsByUserAsync(uid);
        }

        public Task<bool> DeleteItemAsync(string id) => throw new NotImplementedException();

        /// <summary>
        /// Get a collection by id and userid.
        /// TODO: Develop the multi-collection feature.
        /// </summary>
        /// <param name="id">Id of the collection.</param>
        /// <param name="uid">Id of the user.</param>
        /// <returns>Return the selected collection.</returns>
        public async Task<Collection> GetAsync(string collId, bool forceUpdate = false)
        {
            if (collId == null) throw new ArgumentNullException();

            var collection = await App.Database.GetCollectionByIdAsync(collId);
            if (collection == null || forceUpdate)
            {
                if (!this.IsConnected) throw new NotConnectedException();

                var response = await this.Client.GetAsync($"collection/{collId}");
                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode.Equals(HttpStatusCode.NotFound))
                        throw new HttpRequestException(response.ReasonPhrase);
                    return null;
                }
                collection = await Task.Run(() => JsonConvert.DeserializeObject<Collection>(response.Content.ReadAsStringAsync().Result));
                await App.Database.CreateCollectionAsync(collection);
            }

            return collection;
        }

        /// <summary>
        /// Add a card to the colletion.
        /// </summary>
        /// <param name="collId">Collection Id.</param>
        /// <param name="card">Card Entity.</param>
        /// <returns>True for a successful operation.</returns>
        public async Task<bool> AddCardToCollection(string collId, string cardId, bool forceUpdate = false)
        {
            if (collId == null || string.IsNullOrEmpty(cardId)) throw new ArgumentNullException();

            var remote = this.IsConnected && forceUpdate ? await this.RemoteAddToCollection(collId, cardId) : true;

            bool local = false;
            try
            {
                var res = await App.Database.AddCardToCollectionAsync(collId, cardId);
                local = true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(string.Format(AppStrings.ExceptionMessage, e.Message));
            }

            return remote && local;
        }

        /// <summary>
        /// Remove a card from a collection.
        /// </summary>
        /// <param name="collId">Collection Id.</param>
        /// <param name="card">Card Entity.</param>
        /// <returns>True for a successful operation.</returns>
        public async Task<bool> RemoveCardFromCollection(string collId, Card card, bool forceUpdate = false)
        {
            if (collId == null || card == null) throw new ArgumentNullException();

            // Update remote if needed.
            var remote = this.IsConnected && forceUpdate ? await this.RemoteRemoveFromCollection(collId, card.Id) : true;

            // Update local everytime.
            bool local = false;
            try
            {
                var res = App.Database.RemoveCardToCollectionAsync(collId, card.Id);
                local = true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(string.Format(AppStrings.ExceptionMessage, e.Message));
            }

            return remote && local;
        }

        public async Task<bool> RenameCollection(string collId, string newName, bool forceUpdate = false)
        {
            // Update remote if needed.
            var remote = forceUpdate ? await this.RemoteRenameCollection(collId, newName) : true;

            // Update local everytime.
            var res = await App.Database.RenameCollection(collId, newName);
            return remote && res == 0;
        }
    }
}
