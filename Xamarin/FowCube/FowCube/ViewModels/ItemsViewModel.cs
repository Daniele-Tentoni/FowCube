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
        public Command LoadItemsCommand { get; set; }
        public Command DeleteItemCommand { get; set; }

        public ItemsViewModel()
        {
            this.Title = "Browse";
            this.Cards = new ObservableCollection<Card>();
            this.LoadItemsCommand = new Command(async () => await this.ExecuteLoadItemsCommand());
            this.DeleteItemCommand = new Command(async (e) => await this.ExecuteDeleteItemsCommand(e as Card));

            MessagingCenter.Subscribe<NewItemPage, Card>(this, "AddItem", async (obj, item) =>
            {
                var newItem = item as Card;
                this.Cards.Add(newItem);
                var res = await this.DataStore.AddItemAsync(newItem);
            });
        }

        async Task ExecuteLoadItemsCommand()
        {
            if (this.IsBusy)
                return;

            this.IsBusy = true;

            try
            {
                this.Cards.Clear();
                var items = await this.DataStore.GetItemsAsync(true);
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

        async Task ExecuteDeleteItemsCommand(Card e)
        {
            if (this.IsBusy) return;
            this.IsBusy = true;

            try
            {
                var item = await this.DataStore.DeleteItemAsync(e.Id);
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