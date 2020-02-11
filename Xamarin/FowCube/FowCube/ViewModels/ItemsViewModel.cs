namespace FowCube.ViewModels
{
    using FowCube.Models.Cards;
    using Xamarin.Forms;
    using System.Threading.Tasks;
    using System;
    using System.Diagnostics;
    using FowCube.Models.Collection;
    using FowCube.Utils;
    using FowCube.Utils.Strings;

    public class ItemsViewModel : BaseViewModel
    {
        /// <summary>
        /// The cards collection.
        /// </summary>
        // public ObservableCollection<Card> Cards => this.SelectedCollection.CardsIn;

        /// <summary>
        ///  The selected collection.
        /// </summary>
        private readonly string selectedCollectionId;

        /// <summary>
        /// Return the selected collection.
        /// </summary>
        public Collection SelectedCollection => App.Database.GetCollectionByIdAsync(this.selectedCollectionId).Result;

        /// <summary>
        /// Get if a collection is selected.
        /// </summary>
        public bool IsCollectionSelected => !string.IsNullOrEmpty(this.selectedCollectionId);

        public string CollectionTitle => this.IsCollectionSelected ? $"Cards of {this.SelectedCollection.Name}" : this.Title;

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
            this.Title = AppStrings.PageTitleCards;
            this.selectedCollectionId = collectionId;

            // Cards commands.
            this.GetCardsCommand = new Command(async () => await this.ExecuteLoadCardsCommand(false));
            this.UpdateCardsCommand = new Command(async () => await this.ExecuteLoadCardsCommand(true));
            this.DeleteCardCommand = new Command(async (e) => await this.ExecuteDeleteCardCommand(e as Card));

            // Collections commands.
            this.RenameCollectionCommand = new Command(async () => await this.ExecuteRenameCollectionCommand());

            this.UnloadMessageCenterSubscriptions();

            MessagingCenter.Subscribe<AddCardToCollectionViewModel, bool>(this, "NeedReload", async (obj, item) =>
            {
                if (item)
                {
                    await Task.Run(() => this.ExecuteLoadCardsCommand());
                }
            });
        }

        async Task ExecuteLoadCardsCommand(bool forceUpdate = false)
        {
            if (this.IsBusy || this.SelectedCollection == null || this.SelectedCollection.Id == "")
                return;

            this.IsBusy = true;

            try
            {
                // this.Cards.Clear();
                var collection = await this.CollectionsStore.GetAsync(this.SelectedCollection.Id, forceUpdate);
                if (collection == null) return;

                this.Title = string.Format(AppStrings.PageTitleCollectionCards, collection.Name);

                // var items = await this.CardStore.GetItemsAsync(true);
                /* foreach (var item in collection.CardsIn)
                {
                    if (!this.Cards.Contains(new Card { Id = item }))
                    {
                        var card = await this.CardsStore.GetItemAsync(item);
                        if (card != null)
                        {
                            this.Cards.Add(card);
                        }
                    }
                }*/
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
                var res = await this.CollectionsStore.RemoveCardFromCollection(this.selectedCollectionId, e);
                if(res == true)
                {
                    // this.Cards.Remove(e);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format(AppStrings.ExceptionMessage, ex.Message));
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
                    MessagingCenter.Send(this, "Rename", this.selectedCollectionId);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(string.Format(AppStrings.ExceptionMessage, e.Message));
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        public void UnloadMessageCenterSubscriptions()
        {
            MessagingCenter.Unsubscribe<AddCardToCollectionViewModel, Card>(this, "NeedReload");
        }
    }
}