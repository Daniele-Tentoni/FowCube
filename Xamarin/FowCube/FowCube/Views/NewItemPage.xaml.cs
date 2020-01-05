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
    public partial class AddCardToCollectionPage : ContentPage
    {
        public AddCardToCollectionPage(Collection selected)
        {
            this.InitializeComponent();

            this.BindingContext = new AddCardToCollectionViewModel(selected.Name);
        }

        async void Cancel_Clicked(object sender, EventArgs e) => _ = await this.Navigation.PopModalAsync();
    }
}