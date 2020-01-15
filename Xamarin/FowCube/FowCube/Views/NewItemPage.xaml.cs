namespace FowCube.Views
{
    using FowCube.Models.Collection;
    using FowCube.ViewModels;
    using System;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using Xamarin.Forms;

    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class AddCardToCollectionPage : ContentPage
    {
        private readonly AddCardToCollectionViewModel viewModel;

        public AddCardToCollectionPage(Collection selected)
        {
            this.InitializeComponent();

            this.BindingContext = this.viewModel = new AddCardToCollectionViewModel(selected.Name);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (this.viewModel.Cards.Count == 0)
                this.viewModel.LoadCardsCommand.Execute(null);
        }

        async void Cancel_Clicked(object sender, EventArgs e) => _ = await this.Navigation.PopModalAsync();

        private void CardsListView_ItemSelected(object sender, SelectedItemChangedEventArgs e) => Task.Run(() => this.viewModel.ExecuteSelectedCardCommand(false));
    }
}