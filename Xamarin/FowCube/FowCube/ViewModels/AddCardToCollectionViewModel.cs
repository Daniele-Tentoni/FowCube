namespace FowCube.ViewModels
{
    using FowCube.Models;
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Xamarin.Forms;

    public class AddCardToCollectionViewModel : BaseViewModel
    {
        /// <summary>
        /// The local card collection.
        /// </summary>
        public ObservableCollection<Card> Cards { get; set; }

        /// <summary>
        /// Load locally the cards.
        /// TODO: Filter by format.
        /// </summary>
        public Command LoadCardsCommand { get; set; }

        public ICommand SelectedCommand { get; set; }

        /// <summary>
        /// The user selected card inside the controller.
        /// </summary>
        public Card SelectedCard { get; set; }

        public AddCardToCollectionViewModel(string collection)
        {
            // Change the title accordly to the collection I want to modify.
            this.Title = $"Add Card to {collection}";

            // Load all the cards. TODO: Load only when neccessary.
            this.Cards = new ObservableCollection<Card>();
            try
            {
                var cards = this.CardStore.GetItemsAsync().Result;
                if (cards != null)
                {
                    foreach (var card in cards)
                    {
                        this.Cards.Add(card);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            this.SelectedCommand = new Command(async () => await this.ExecuteSelectedCardCommand());
        }

        async Task ExecuteSelectedCardCommand()
        {
            MessagingCenter.Send(this, "AddItem", this.SelectedCard);
            await Application.Current.MainPage.Navigation.PopModalAsync();
        }
    }
}
