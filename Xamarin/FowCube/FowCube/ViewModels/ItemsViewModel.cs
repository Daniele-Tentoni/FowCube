namespace FowCube.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Threading.Tasks;

    using Xamarin.Forms;
    using FowCube.Views;
    using FowCube.Models;

    public class ItemsViewModel : BaseViewModel
    {
        /// <summary>
        /// The cards collection.
        /// </summary>
        public ObservableCollection<Card> Cards { get; set; }
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

            MessagingCenter.Subscribe<NewItemPage, Card>(this, "AddItem", async (obj, item) =>
            {
                var newItem = item as Card;
                this.Cards.Add(newItem);
                var res = await this.CardStore.AddItemAsync(newItem);
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
                var collection = await this.CollectionsStore.GetAsync("1", this.authInfo.GetAuthenticatedUid());
                if (collection == null)
                {
                    var res = await this.CollectionsStore.CreateAsync("1", this.authInfo.GetAuthenticatedUid());
                    if(res)
                    {
                        collection = await this.CollectionsStore.GetAsync("1", this.authInfo.GetAuthenticatedUid());
                    }
                }

                var items = await this.CardStore.GetItemsAsync(true);
                foreach (var item in items)
                {
                    this.Cards.Add(item);
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