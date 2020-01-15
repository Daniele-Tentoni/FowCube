namespace FowCube.ViewModels
{
    using FowCube.Models;
    using FowCube.Models.Collection;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Xamarin.Essentials;
    using Xamarin.Forms;

    public class MenuPageViewModel: BaseViewModel
    {
        public List<BasicCollection> Collections { get; set; }

        public ObservableCollection<HomeMenuItem> MenuItems { get; set; }
        public Command LoadMenuItemsCommand { get; set; }

        public MenuPageViewModel()
        {
            this.Title = "About";
            this.Collections = new List<BasicCollection>();
            this.MenuItems = new ObservableCollection<HomeMenuItem>();
            this.LoadMenuItemsCommand = new Command(async () => await this.ExecuteLoadCollectionsCommand());
        }

        async Task ExecuteLoadCollectionsCommand()
        {
            if (this.IsBusy) return;
            this.IsBusy = true;
            this.MenuItems.Clear();

            try
            {
                this.Collections = new List<BasicCollection>();
                string userId = await SecureStorage.GetAsync("login_id");
                var basicCollections = await this.CollectionsStore.GetAllUserCollectionsAsync(userId);

                basicCollections.ForEach(collection =>
                {
                    this.MenuItems.Add(new HomeMenuItem { MenuType = MenuItemType.Browse, Title = collection.Name, Arg = collection.Id });
                });
                // this.menuItems.Add(new HomeMenuItem { MenuType = MenuItemType.Login, Title = "Login" });
                this.MenuItems.Add(new HomeMenuItem { MenuType = MenuItemType.About, Title = "About" });
            }
            catch (Exception ex)
            {
                // This may occure when api fail twice to load collection.
                Debug.WriteLine(ex);
            }
            finally
            {
                this.IsBusy = false;
            }
        }
    }
}
