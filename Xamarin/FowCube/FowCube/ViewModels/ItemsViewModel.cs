﻿namespace FowCube.ViewModels
{
    using FowCube.Models;
    using System.Collections.ObjectModel;
    using Xamarin.Forms;
    using System.Threading.Tasks;
    using System;
    using System.Diagnostics;
    using FowCube.Models.Collection;

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

        public ItemsViewModel(string collectionId)
        {
            this.Title = "Browse";
            this.Cards = new ObservableCollection<Card>();
            this.SelectedCollection = new Collection() { Id = collectionId };
            this.LoadCardsCommand = new Command(async () => await this.ExecuteLoadCardsCommand());
            this.DeleteCardCommand = new Command(async (e) => await this.ExecuteDeleteCardCommand(e as Card));

            this.UnloadMessageCenterSubscriptions();
            MessagingCenter.Subscribe<AddCardToCollectionViewModel, Card>(this, $"CreateCardTo{this.SelectedCollection.Id}", async (obj, item) =>
            {
                var newItem = item as Card;
                var res = await this.CardStore.AddItemAsync(newItem);
                if(!string.IsNullOrEmpty(res))
                {
                    var added = await this.CollectionsStore.AddCardToCollection(this.SelectedCollection.Id, new Card { Id = res });
                    if (added) await Task.Run(() => this.ExecuteLoadCardsCommand());
                }
            });

            MessagingCenter.Subscribe<AddCardToCollectionViewModel, Card>(this, $"AddCardTo{this.SelectedCollection.Id}", async (obj, item) =>
            {
                var cardIn = item as Card;
                var res = await this.CollectionsStore.AddCardToCollection(this.SelectedCollection.Id, cardIn);
                if (res) await Task.Run(() => this.ExecuteLoadCardsCommand());
            });
        }

        async Task ExecuteLoadCardsCommand()
        {
            if (this.IsBusy || this.SelectedCollection == null || this.SelectedCollection.Id == "")
                return;

            this.IsBusy = true;

            try
            {
                this.Cards.Clear();
                var collection = await this.CollectionsStore.GetAsync(this.SelectedCollection.Id);
                if (collection == null) return;

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

        public void UnloadMessageCenterSubscriptions()
        {
            MessagingCenter.Unsubscribe<AddCardToCollectionViewModel, Card>(this, $"CreateCardTo{this.SelectedCollection.Id}");
            MessagingCenter.Unsubscribe<AddCardToCollectionViewModel, Card>(this, $"AddCardTo{this.SelectedCollection.Id}");
        }
    }
}