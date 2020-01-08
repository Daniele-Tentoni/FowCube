﻿namespace FowCube.ViewModels
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

        /// <summary>
        /// Input the selected card from the input fields to the collection.
        /// </summary>
        public ICommand SelectedNewCardCommand { get; set; }

        /// <summary>
        /// Input the selected card from the list.
        /// </summary>
        public ICommand SelectedOldCardCommand { get; set; }

        /// <summary>
        /// The user selected card inside the controller.
        /// </summary>
        public Card SelectedCard { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public AddCardToCollectionViewModel(string collection)
        {
            // Change the title accordly to the collection I want to modify.
            this.Title = $"Add Card to {collection}";
            this.SelectedCard = new Card();
            this.Cards = new ObservableCollection<Card>();

            this.SelectedNewCardCommand = new Command(async () => await this.ExecuteSelectedCardCommand(true));
            this.SelectedOldCardCommand = new Command(async () => await this.ExecuteSelectedCardCommand(false));
            this.LoadCardsCommand = new Command(async () => await this.ExecuteLoadCardsCommand());
        }

        async Task ExecuteLoadCardsCommand()
        {
            if (this.IsBusy)
                return;

            this.IsBusy = true;

            // Load all the cards. TODO: Load only when neccessary.
            try
            {
                var cards = await this.CardStore.GetItemsAsync(true);
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
            finally
            {
                this.IsBusy = false;
            }
        }

        /// <summary>
        /// Add a new card to the collection and return to the collection page.
        /// </summary>
        /// <param name="newCard">If the card is new or old.</param>
        /// <returns>Nothing.</returns>
        public async Task ExecuteSelectedCardCommand(bool newCard)
        {
            // Add a card to the database before add that to collections.
            if (newCard)
            {
                MessagingCenter.Send(this, "AddItem", new Card { Name = Name, Description = Description });
            }

            MessagingCenter.Send(this, "AddCardToCollection", this.SelectedCard);
            await Application.Current.MainPage.Navigation.PopModalAsync();
        }
    }
}
