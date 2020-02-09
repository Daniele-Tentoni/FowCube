namespace FowCube.Services
{
    using FowCube.Models.Cards;
    using Newtonsoft.Json;
    using Realms;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Xamarin.Forms.Internals;

    public class CardStore : BasicStore
    {
        private string TAG = "CARD STORE";

        /// <summary>
        /// Create the card store with the app endpoint.
        /// </summary>
        public CardStore(): base("app") { }

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

        /// <summary>
        /// Add a card in the local db and to remote servers.
        /// </summary>
        /// <param name="item">Card to add.</param>
        /// <returns>The id of the card.</returns>
        public async Task<string> AddCardAsync(Card item)
        {
            if (item == null) return null;

            // TODO: This id is not correct.
            string cardId = string.Empty;
            if(this.IsConnected)
            {
                var serializedItem = JsonConvert.SerializeObject(item);
                var response = await this.Client.PostAsync($"card", new StringContent(serializedItem, Encoding.UTF8, "application/json"));
                if (response.IsSuccessStatusCode)
                    cardId = await response.Content.ReadAsStringAsync();
            }

            item.Id = cardId;
            try {
                this.Realm.Write(() => this.Realm.Add(item));
            } catch (Exception e) {
                Log.Warning(this.TAG, $"Exception thrown: {e.Message}");
            }

            var card = this.Realm.Find<Card>(item.Id);
            return cardId;
        }

        /// <summary>
        /// Delete a card from the local db and from remote servers.
        /// This scenario will not be real, a user cannot delete a card.
        /// </summary>
        /// <param name="id">Id of the card to remove.</param>
        /// <returns>True if success, false if fail.</returns>
        public async Task<bool> DeleteItemAsync(string id)
        {
            bool remote = true, local = true;

            // If there is the connection, I'll delete the card in remote too.
            if (!string.IsNullOrEmpty(id) && this.IsConnected)
                remote = (await this.Client.DeleteAsync($"card/{id}")).IsSuccessStatusCode;

            // I try to remove the card from Realm too.
            var card = this.Realm.Find<Card>(id);
            if(card != null)
            {
                try {
                    using (var trans = this.Realm.BeginWrite()) {
                        this.Realm.Remove(card);
                        trans.Commit();
                    }
                } catch (Exception e) {
                    Log.Warning(this.TAG, $"Exception thrown: {e.Message}");
                    local = false;
                }
            }

            return remote && local;
        }

        /// <summary>
        /// Search the card in the local db. If it fail, search it with APIs.
        /// </summary>
        /// <param name="id">Id of selected card.</param>
        /// <returns>Card.</returns>
        public async Task<Card> GetItemAsync(string id)
        {
            var card = this.Realm.Find<Card>(id); // This is more fast.

            if (card == null && id != null && this.IsConnected)
            {
                try {
                    var json = await this.Client.GetStringAsync($"card/{id}");
                    card = await Task.Run(() => JsonConvert.DeserializeObject<Card>(json));
                    this.Realm.Write(() => this.Realm.Add<Card>(card));
                } catch (Exception e) {
                    Log.Warning(TAG, $"Exception thrown: {e.Message}");
                }
            }

            return card;
        }

        /// <summary>
        /// Returns all card in the local db. Download the collection when the user reload the list.
        /// </summary>
        /// <param name="forceRefresh">Get from APIs all the collection.</param>
        /// <returns>List of cards. Can be empty.</returns>
        public async Task<IEnumerable<Card>> GetItemsAsync(bool forceRefresh = false)
        {
            if (forceRefresh && this.IsConnected)
            {
                var json = await this.Client.GetStringAsync($"card");
                var cards = await Task.Run(() => JsonConvert.DeserializeObject<IEnumerable<Card>>(json));
                this.Realm.Write(() => cards.ToList().ForEach(elem => this.Realm.Add(elem, true)));
                return cards;
            }
            
            return this.Realm.All<Card>();
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
