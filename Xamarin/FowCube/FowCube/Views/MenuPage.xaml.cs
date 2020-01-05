namespace FowCube.Views
{
    using FowCube.Models;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Xamarin.Forms;

    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MenuPage : ContentPage
    {
        MainPage RootPage { get => Application.Current.MainPage as MainPage; }
        private readonly List<HomeMenuItem> menuItems;

        public MenuPage()
        {
            this.InitializeComponent();

            this.menuItems = new List<HomeMenuItem>
            {
                new HomeMenuItem {Id = MenuItemType.Browse, Title="Browse" },
                new HomeMenuItem {Id = MenuItemType.Login, Title = "Login" },
                new HomeMenuItem {Id = MenuItemType.About, Title="About" }
            };

            this.ListViewMenu.ItemsSource = this.menuItems;

            this.ListViewMenu.SelectedItem = this.menuItems[0];
            this.ListViewMenu.ItemSelected += async (sender, e) =>
            {
                if (e.SelectedItem == null)
                    return;

                var id = (int)((HomeMenuItem)e.SelectedItem).Id;
                await this.RootPage.NavigateFromMenu(id);
            };
        }
    }
}