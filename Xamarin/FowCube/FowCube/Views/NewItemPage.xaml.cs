namespace FowCube.Views
{
    using System;
    using System.ComponentModel;
    using Xamarin.Forms;

    using FowCube.Models;

    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class NewItemPage : ContentPage
    {
        public Card Item { get; set; }

        public NewItemPage()
        {
            this.InitializeComponent();

            this.Item = new Card
            {
                Name = "Card name",
                Description = "This is a card description."
            };

            this.BindingContext = this;
        }

        async void Save_Clicked(object sender, EventArgs e)
        {
            MessagingCenter.Send(this, "AddItem", this.Item);
            await this.Navigation.PopModalAsync();
        }

        async void Cancel_Clicked(object sender, EventArgs e) => _ = await this.Navigation.PopModalAsync();
    }
}