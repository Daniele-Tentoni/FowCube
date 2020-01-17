namespace FowCube.ViewModels
{
    using FowCube.Models;
    using FowCube.Models.Collection;
    using FowCube.Views;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using Xamarin.Essentials;
    using Xamarin.Forms;
    using Xamarin.Forms.Internals;

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

                int id = 0;
                basicCollections.ForEach(collection =>
                {
                    this.MenuItems.Add(new HomeMenuItem { Id = id++, MenuType = MenuItemType.Browse, Title = collection.Name, Arg = collection.Id });
                });
                // this.menuItems.Add(new HomeMenuItem { MenuType = MenuItemType.Login, Title = "Login" });
                this.MenuItems.Add(new HomeMenuItem { Id = id++, MenuType = MenuItemType.About, Title = "About" });

                if(this.MenuItems.Count(c => c.MenuType == MenuItemType.Browse) > 0)
                {
                    // After this operation I exit, so I have to set IsBusy at false at the moment.
                    this.IsBusy = false;
                    await this.ExecuteMenuSelectionCommand(this.MenuItems.First(f => f.MenuType == MenuItemType.Browse));
                }
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

        public async Task ExecuteMenuSelectionCommand(HomeMenuItem menuItem)
        {
            if (this.IsBusy) return;
            this.IsBusy = true;

            try
            {
                // Navigate to the selected menu voice.
                if (menuItem == null && Application.Current.MainPage is MainPage)
                    return;

                await ((MainPage)Application.Current.MainPage).NavigateFromMenu(menuItem);
            }
            catch (Exception ex)
            {
                Log.Warning("MENU NAVIGATION", $"Exception: {ex.StackTrace}");
            }
        }
    }
}
