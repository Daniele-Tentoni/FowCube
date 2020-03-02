﻿namespace FowCube.ViewModels
{
    using FowCube.Models.Cards;
    using FowCube.Models.Collection;
    using FowCube.Utils.Strings;
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Xamarin.Forms;

    public class AddCardToCollectionViewModel : BaseViewModel
    {
        /// <summary>
        /// The local card collection.
        /// </summary>
        public ObservableCollection<Card> Cards => new ObservableCollection<Card>(this.SelectedCollection.CardsIn);
                    

        private readonly string selectedCollectionId;
        public Collection SelectedCollection => App.Database.GetCollectionByIdAsync(this.selectedCollectionId).Result;

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

        public AddCardToCollectionViewModel(Collection selected)
        {
            this.selectedCollectionId = selected.Id;

            // Change the title accordly to the collection I want to modify.
            this.Title = string.Format(AppStrings.PageTitleAddCards, this.SelectedCollection.Name);
            this.SelectedCard = new Card();

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
                var cards = await App.Database.GetAllCards(); // .GetItemsAsync(true);
                if (cards.Count() == this.Cards.Count)
                    await Device.InvokeOnMainThreadAsync(() =>
                     {
                         Application.Current.MainPage?.DisplayAlert("Right", "Loaded", "Ok");
                     });
                else
                    await Device.InvokeOnMainThreadAsync(() =>
                     {
                         Application.Current.MainPage?.DisplayAlert("Error", "Not loaded", "Bad.");
                     });
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
            if (newCard)
            {
                string cardId = Guid.NewGuid().ToString();
                // If it's a new card, I'll add it to the database before.
                // var res = await this.CardsStore.AddCardAsync(new Card
                var res = await App.Database.CreateCardAsync(new Card
                {
                    Id = cardId,
                    Name = Name,
                    Description = Description
                });

                if (res > 0)
                {
                    var added = await App.Database.AddCardToCollectionAsync(this.selectedCollectionId, cardId);
                    MessagingCenter.Send(this, "NeedReload", added);
                }
            }
            else
            {
                // If it's an old card, I'll add it to collection instead.
                // TODO: This dosen't work and I don't know why.
                // var res = await this.CollectionsStore.AddCardToCollection(this.selectedCollectionId, this.SelectedCard.Id);
                try
                {
                    // var added = await this.CollectionsStore.AddCardToCollection(this.selectedCollectionId, this.SelectedCard.Id);
                    /*using(var trans = this.realm.BeginWrite())
                    {
                        // Search all entities in Realm Database.
                        var card = this.realm.Find<Card>();
                        // this.realm.Find<Collection>(this.selectedCollectionId).CardsIn.Add(card);
                        // Search only the card in Realm Database.
                        this.SelectedCollection.CardsIn.Add(card);
                        trans.Commit();
                    }*/
                }
                catch (Exception e)
                {
                    Debug.WriteLine(AppStrings.ExceptionMessage, e.Message);
                }
                MessagingCenter.Send(this, "NeedReload", true);
            }

            await Application.Current.MainPage.Navigation.PopModalAsync();
        }
    }
}