namespace FowCube.Views
{
    using System;
    using System.ComponentModel;
    using Xamarin.Forms;

    using FowCube.Models.Cards;
    using FowCube.ViewModels;

    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class ItemsPage : ContentPage
    {
        readonly ItemsViewModel viewModel;

        public ItemsPage(string collectionId)
        {
            this.InitializeComponent();
            this.BindingContext = this.viewModel = new ItemsViewModel(collectionId);
        }

        public ItemsPage() : this(string.Empty) { }

        async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            if (!(args.SelectedItem is Card item))
                return;

            // Go to the card detail page.
            await this.Navigation.PushAsync(new NavigationPage(new ItemDetailPage(new ItemDetailViewModel(item))));

            // Manually deselect item.
            this.CardsListView.SelectedItem = null;
        }

        async void AddItem_Clicked(object sender, EventArgs e) => await this.Navigation.PushModalAsync(new NavigationPage(new AddCardToCollectionPage(this.viewModel.SelectedCollection)));

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // At startup load cards in the view.
            // if (this.viewModel.IsCollectionSelected)
                // this.viewModel.GetCardsCommand.Execute(null);
        }
    }
}