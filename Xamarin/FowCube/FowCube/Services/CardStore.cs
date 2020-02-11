namespace FowCube.Services
{
    using FowCube.Models.Cards;
    using Newtonsoft.Json;
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
            string cardId = Guid.NewGuid().ToString();
            if(this.IsConnected)
            {
                var serializedItem = JsonConvert.SerializeObject(item);
                var response = await this.Client.PostAsync($"card", new StringContent(serializedItem, Encoding.UTF8, "application/json"));
                if (response.IsSuccessStatusCode)
                    cardId = await response.Content.ReadAsStringAsync();
            }

            item.Id = cardId;
            try {
                await App.Database.CreateCardAsync(item);
            } catch (Exception e) {
                Log.Warning(this.TAG, $"Exception thrown: {e.Message}");
            }

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
            try
            {
                var res = await App.Database.DeleteCardByIdAsync(id);
            }
            catch (Exception e)
            {
                Log.Warning(this.TAG, $"Exception thrown: {e.Message}");
                local = false;
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
            var card = await App.Database.GetCardByIdAsync(id); // This is more fast.

            if (card == null && id != null && this.IsConnected)
            {
                try
                {
                    var json = await this.Client.GetStringAsync($"card/{id}");
                    card = await Task.Run(() => JsonConvert.DeserializeObject<Card>(json));
                    var res = await App.Database.CreateCardAsync(card);
                }
                catch (Exception e)
                {
                    Log.Warning(this.TAG, $"Exception thrown: {e.Message}");
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
                cards.ForEach(async card =>
                {
                    var res = await App.Database.CreateCardAsync(card);
                    Log.Warning(this.TAG, $"Added: {res}");
                });
                return cards;
            }

            return await App.Database.GetAllCards();
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
