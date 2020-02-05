namespace FowCube.Views
{
    using System.ComponentModel;
    using Xamarin.Forms;

    using FowCube.Models.Cards;
    using FowCube.ViewModels;

    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class ItemDetailPage : ContentPage
    {
        ItemDetailViewModel viewModel;

        public ItemDetailPage(ItemDetailViewModel viewModel)
        {
            this.InitializeComponent();

            this.BindingContext = this.viewModel = viewModel;
        }

        /// <summary>
        /// Constructor without arguments. Never use this.
        /// </summary>
        public ItemDetailPage()
        {
            this.InitializeComponent();

            var item = new Card
            {
                Name = "Item 1",
                Description = "This is an item description."
            };

            this.viewModel = new ItemDetailViewModel(item);
            this.BindingContext = this.viewModel;
        }
    }
}