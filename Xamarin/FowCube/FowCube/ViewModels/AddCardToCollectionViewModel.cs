namespace FowCube.ViewModels
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
        public ObservableCollection<Card> Cards => new ObservableCollection<Card>(this.realm.All<Card>().ToList());

        private readonly string selectedCollectionId;
        public Collection SelectedCollection => this.realm.Find<Collection>(this.selectedCollectionId);

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
                var cards = await this.CardsStore.GetItemsAsync(true);
                var cardList = this.realm.All<Card>().ToList();
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
                // If it's a new card, I'll add it to the database before.
                var res = await this.CardsStore.AddCardAsync(new Card
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = Name,
                    Description = Description
                });
                if (!string.IsNullOrEmpty(res))
                {
                    var added = await this.CollectionsStore.AddCardToCollection(this.selectedCollectionId, res);
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
                    this.realm.Write(() =>
                    {
                        var card = this.realm.Find<Card>(this.SelectedCard.Id);
                        this.realm.Find<Collection>(this.selectedCollectionId).CardsIn.Add(card);
                    });
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