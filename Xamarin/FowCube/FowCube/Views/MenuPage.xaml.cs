namespace FowCube.Views
{
    using FowCube.Models.HomeMenuItems;
    using FowCube.ViewModels;
    using System;
    using System.ComponentModel;
    using Xamarin.Forms;
    using Xamarin.Forms.Internals;

    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MenuPage : ContentPage
    {
        private readonly MenuPageViewModel viewModel;

        public MenuPage()
        {
            this.InitializeComponent();
            try
            {
                this.BindingContext = this.viewModel = new MenuPageViewModel();
            }
            catch (Exception e)
            {
                Log.Warning("PROBLEM", $"{e.Message}: {e.StackTrace}");
            }

            // this.ListViewMenu.SelectedItem = this.viewModel.MenuItems[0];
            this.ListViewMenu.ItemSelected += this.ViewMenu_ItemSelected;
        }

        private async void ViewMenu_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e == null) return;
            await this.viewModel.ExecuteMenuSelectionCommand((HomeMenuItem)e.SelectedItem);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (this.viewModel.MenuItems.Count == 0) 
                this.viewModel.LoadMenuItemsCommand.Execute(null);
        }
    }
}