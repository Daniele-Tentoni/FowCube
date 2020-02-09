namespace FowCube.ViewModels
{
    using FowCube.Models.Cards;
    using System.Collections.ObjectModel;
    using Xamarin.Forms;
    using System.Threading.Tasks;
    using System;
    using System.Diagnostics;
    using FowCube.Models.Collection;
    using FowCube.Utils;

    public class ItemsViewModel : BaseViewModel
    {
        /// <summary>
        /// The cards collection.
        /// </summary>
        public ObservableCollection<Card> Cards { get; set; }

        /// <summary>
        ///  The selected collection.
        /// </summary>
        private Collection selectedCollection;

        /// <summary>
        /// Return the selected collection.
        /// </summary>
        public Collection SelectedCollection
        {
            get { return this.selectedCollection; }
            private set
            {
                this.SetProperty(ref this.selectedCollection, value);
                this.OnPropertyChanged(nameof(this.CollectionTitle));
            }
        }

        /// <summary>
        /// Get if a collection is selected.
        /// </summary>
        public bool IsCollectionSelected => this.selectedCollection != null && !string.IsNullOrEmpty(this.selectedCollection.Id);

        public string CollectionTitle => this.IsCollectionSelected ? $"Cards of {this.selectedCollection.Name}" : this.Title;

        /// <summary>
        /// Get the cards without upload the list.
        /// </summary>
        public Command GetCardsCommand { get; set; }

        /// <summary>
        /// Get the cards and upload the list.
        /// </summary>
        public Command UpdateCardsCommand { get; set; }
        public Command DeleteCardCommand { get; set; }

        public Command RenameCollectionCommand { get; set; }

        public ItemsViewModel(string collectionId)
        {
            this.Title = "Cards";
            this.Cards = new ObservableCollection<Card>();
            this.SelectedCollection = new Collection() { Id = collectionId };

            // Cards commands.
            this.GetCardsCommand = new Command(async () => await this.ExecuteLoadCardsCommand(false));
            this.UpdateCardsCommand = new Command(async () => await this.ExecuteLoadCardsCommand(true));
            this.DeleteCardCommand = new Command(async (e) => await this.ExecuteDeleteCardCommand(e as Card));

            // Collections commands.
            this.RenameCollectionCommand = new Command(async () => await this.ExecuteRenameCollectionCommand());

            this.UnloadMessageCenterSubscriptions();
            MessagingCenter.Subscribe<AddCardToCollectionViewModel, Card>(this, Consts.CREATECARDMESSAGE, async (obj, item) =>
            {
                var newItem = item as Card;
                var res = await this.CardsStore.AddCardAsync(newItem);
                if(!string.IsNullOrEmpty(res))
                {
                    var added = await this.CollectionsStore.AddCardToCollection(this.SelectedCollection.Id, new Card { Id = res });
                    if (added) await Task.Run(() => this.ExecuteLoadCardsCommand());
                }
            });

            MessagingCenter.Subscribe<AddCardToCollectionViewModel, Card>(this, Consts.ADDCARDMESSAGE, async (obj, item) =>
            {
                var cardIn = item as Card;
                var res = await this.CollectionsStore.AddCardToCollection(this.SelectedCollection.Id, cardIn);
                if (res) await Task.Run(() => this.ExecuteLoadCardsCommand());
            });
        }

        async Task ExecuteLoadCardsCommand(bool forceUpdate = false)
        {
            if (this.IsBusy || this.SelectedCollection == null || this.SelectedCollection.Id == "")
                return;

            this.IsBusy = true;

            try
            {
                this.Cards.Clear();
                var collection = await this.CollectionsStore.GetAsync(this.SelectedCollection.Id, forceUpdate);
                if (collection == null) return;

                this.Title = "Cards of" + collection.Name;
                this.SelectedCollection = collection;

                // var items = await this.CardStore.GetItemsAsync(true);
                foreach (var item in collection.CardsIn)
                {
                    if (!this.Cards.Contains(new Card { Id = item }))
                    {
                        var card = await this.CardsStore.GetItemAsync(item);
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
                var res = await this.CollectionsStore.RemoveCardFromCollection(this.SelectedCollection.Id, e);
                if(res == true)
                {
                    this.Cards.Remove(e);
                }
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
        /// Execute the renaming of the collection.
        /// </summary>
        /// <returns>Nothing.</returns>
        async Task ExecuteRenameCollectionCommand()
        {
            if (this.IsBusy || !this.IsCollectionSelected) return;
            this.IsBusy = true;

            string result = await Device.InvokeOnMainThreadAsync(async () =>
            {
                return await Application.Current.MainPage.DisplayPromptAsync("Rename collection", "What's the name?");
            });

            try
            {
                var res = await this.CollectionsStore.RenameCollection(this.SelectedCollection.Id, result);
                if (res == true)
                {
                    // this.SelectedCollection.Name = result;
                    this.Title = Consts.GetCardsPageTitle(result);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Exception thrown: {e.Message}");
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        public void UnloadMessageCenterSubscriptions()
        {
            MessagingCenter.Unsubscribe<AddCardToCollectionViewModel, Card>(this, Consts.CREATECARDMESSAGE);
            MessagingCenter.Unsubscribe<AddCardToCollectionViewModel, Card>(this, Consts.ADDCARDMESSAGE);
        }
    }
}