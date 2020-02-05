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
    using System.Net.Http;
    using System.Threading.Tasks;
    using Xamarin.Forms;
    using Xamarin.Forms.Internals;

    public class MenuPageViewModel : BaseViewModel
    {
        public ObservableCollection<HomeMenuItem> CollectionsMenuItems { get; set; }
        public ObservableCollection<HomeMenuItem> MenuItems { get; set; }
        public Command LoadMenuItemsCommand { get; set; }

        public MenuPageViewModel()
        {
            this.Title = "About";
            this.CollectionsMenuItems = new ObservableCollection<HomeMenuItem>();
            this.MenuItems = new ObservableCollection<HomeMenuItem>();
            this.LoadMenuItemsCommand = new Command(async () => await this.ExecuteLoadCollectionsCommand());
        }

        async Task ExecuteLoadCollectionsCommand()
        {
            if (this.IsBusy) return;
            this.IsBusy = true;
            this.CollectionsMenuItems.Clear();
            this.MenuItems.Clear();

            try
            {
                int id = 0;
                this.MenuItems.Add(new HomeMenuItem { Id = id++, MenuType = MenuItemType.Settings, Title = "Settings" });
                this.MenuItems.Add(new HomeMenuItem { Id = id++, MenuType = MenuItemType.About, Title = "About" });
                this.MenuItems.Add(new HomeMenuItem { Id = id++, MenuType = MenuItemType.Logout, Title = "Logout" });

                var basicCollections = new List<BasicCollection>();
                try
                {
                    basicCollections = await this.CollectionsStore.GetAllUserCollectionsAsync(this.UserId);
                }
                catch (HttpRequestException)
                {
                    var res = await this.CollectionsStore.CreateAsync("1", this.UserId);
                    if (res != null)
                    {
                        basicCollections = await this.CollectionsStore.GetAllUserCollectionsAsync(this.UserId);
                    }
                }

                if(basicCollections.Count == 0)
                {
                    var res = await this.CollectionsStore.CreateAsync("1", this.UserId);
                    if (res != null)
                    {
                        basicCollections = await this.CollectionsStore.GetAllUserCollectionsAsync(this.UserId);
                    }
                }
                basicCollections.ForEach(collection =>
                {
                    this.CollectionsMenuItems.Add(new HomeMenuItem { Id = id++, MenuType = MenuItemType.Browse, Title = collection.Name, Arg = collection.Id });
                });

                if (basicCollections.Count > 0)
                {
                    // After this operation I exit, so I have to set IsBusy at false at the moment.
                    this.IsBusy = false;
                    await this.ExecuteMenuSelectionCommand(this.CollectionsMenuItems.First());
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
                if (menuItem == null || !(Application.Current.MainPage is MainPage))
                    return;

                this.IsBusy = false;
                await ((MainPage)Application.Current.MainPage).NavigateFromMenu(menuItem);
            }
            catch (Exception ex)
            {
                Log.Warning("MENU NAVIGATION", $"Exception: {ex.StackTrace}");
            }
            finally
            {
                this.IsBusy = false;
            }
        }
    }
}
