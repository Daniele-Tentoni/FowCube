namespace FowCube.ViewModels
{
    using FowCube.Views;
    using FowCube.Models;
    using System.Collections.ObjectModel;
    using Xamarin.Forms;
    using System.Threading.Tasks;
    using System;
    using System.Diagnostics;
    using System.Collections.Generic;

    public class ItemsViewModel : BaseViewModel
    {
        /// <summary>
        /// The cards collection.
        /// </summary>
        public ObservableCollection<Card> Cards { get; set; }
        public Collection SelectedCollection { get; set; }
        public Command LoadCardsCommand { get; set; }
        public Command EditCardCommand { get; set; }
        public Command DeleteCardCommand { get; set; }

        public ItemsViewModel()
        {
            this.Title = "Browse";
            this.Cards = new ObservableCollection<Card>();
            this.LoadCardsCommand = new Command(async () => await this.ExecuteLoadCardsCommand());
            this.EditCardCommand = new Command(async (e) => await this.ExecuteEditCardCommand(e as Card));
            this.DeleteCardCommand = new Command(async (e) => await this.ExecuteDeleteCardCommand(e as Card));

            MessagingCenter.Subscribe<AddCardToCollectionViewModel, Card>(this, "AddItem", async (obj, item) =>
            {
                var newItem = item as Card;
                var res = await this.CardStore.AddItemAsync(newItem);
            });

            MessagingCenter.Subscribe<AddCardToCollectionViewModel, Card>(this, "AddCardToCollection", async (obj, item) =>
            {
                var cardIn = item as Card;
                var res = await this.CollectionsStore.AddCardToCollection(this.SelectedCollection.Id, cardIn);
                if (res) await Task.Run(() => this.ExecuteLoadCardsCommand());
            });
        }

        async Task ExecuteLoadCardsCommand()
        {
            if (this.IsBusy)
                return;

            this.IsBusy = true;

            try
            {
                this.Cards.Clear();
                var collection = await this.CollectionsStore.GetAsync("1", this.AuthInfo.GetAuthenticatedUid());
                if (collection == null)
                {
                    var res = await this.CollectionsStore.CreateAsync("1", this.AuthInfo.GetAuthenticatedUid());
                    if(res != null)
                    {
                        collection = await this.CollectionsStore.GetAsync("1", this.AuthInfo.GetAuthenticatedUid());
                        if(collection == null)
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                Application.Current.MainPage.DisplayAlert("Alert", "No collection found.", "OK");
                            });
                        }
                    }
                }
                this.Title = collection.Name;
                this.SelectedCollection = collection;

                // var items = await this.CardStore.GetItemsAsync(true);
                foreach (var item in collection.CardsIn)
                {
                    if (!this.Cards.Contains(new Card { Id = item }))
                    {
                        var card = await this.CardStore.GetItemAsync(item);
                        if (card != null)
                        {
                            this.Cards.Add(card);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // This may occure when api fail twice to load collection.
                Debug.WriteLine(ex);
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        /// <summary>
        /// If is not busy, open the Edit Card View. Not implemented yet.
        /// </summary>
        /// <param name="e">Card to update.</param>
        /// <returns>Nothing</returns>
        async Task ExecuteEditCardCommand(Card e)
        {
            if (this.IsBusy) return;
            this.IsBusy = true;

            try
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Application.Current.MainPage.DisplayAlert("Alert", "Not implemented yet.", "OK");
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        /// <summary>
        /// If is not busy, call the DataStore to delete the card.
        /// </summary>
        /// <param name="e">Card to delete.</param>
        /// <returns>Nothing.</returns>
        async Task ExecuteDeleteCardCommand(Card e)
        {
            if (this.IsBusy) return;
            this.IsBusy = true;

            try
            {
                var item = await this.CardStore.DeleteItemAsync(e.Id);
                this.Cards.Remove(e);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                this.IsBusy = false;
            }
        }
    }
}