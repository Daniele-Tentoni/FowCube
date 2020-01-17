namespace FowCube.Views
{
    using FowCube.Models;
    using FowCube.Services;
    using FowCube.ViewModels;
    using System.ComponentModel;
    using System.Linq;
    using Xamarin.Forms;

    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MenuPage : ContentPage
    {
        MainPage RootPage { get => Application.Current.MainPage as MainPage; }
        public CollectionStore CollectionsStore => new CollectionStore();
        private readonly MenuPageViewModel viewModel;

        public MenuPage()
        {
            this.InitializeComponent();
            this.BindingContext = this.viewModel = new MenuPageViewModel();

            // this.ListViewMenu.SelectedItem = this.viewModel.MenuItems[0];
            this.ListViewMenu.ItemSelected += async (sender, e) =>
            {
                if (e == null) return;
                await this.viewModel.ExecuteMenuSelectionCommand((HomeMenuItem)e.SelectedItem);
            };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (this.viewModel.MenuItems.Count == 0)
                this.viewModel.LoadMenuItemsCommand.Execute(null);
        }
    }
}