namespace FowCube.Views
{
    using System;
    using System.ComponentModel;
    using Xamarin.Forms;

    using FowCube.Models;
    using FowCube.ViewModels;

    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class ItemsPage : ContentPage
    {
        readonly ItemsViewModel viewModel;

        public ItemsPage()
        {
            this.InitializeComponent();

            this.BindingContext = this.viewModel = new ItemsViewModel();
        }

        async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            if (!(args.SelectedItem is Card item))
                return;

            await this.Navigation.PushAsync(new ItemDetailPage(new ItemDetailViewModel(item)));

            // Manually deselect item.
            this.CardsListView.SelectedItem = null;
        }

        async void AddItem_Clicked(object sender, EventArgs e) => await this.Navigation.PushModalAsync(new NavigationPage(new NewItemPage()));

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (this.viewModel.Cards.Count == 0)
                this.viewModel.LoadCardsCommand.Execute(null);
        }
    }
}