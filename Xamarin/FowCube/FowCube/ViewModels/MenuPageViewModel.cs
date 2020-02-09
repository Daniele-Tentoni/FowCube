namespace FowCube.ViewModels
{
    using FowCube.Models.Collection;
    using FowCube.Models.HomeMenuItems;
    using FowCube.Utils;
    using FowCube.Views;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Xamarin.Forms;
    using Xamarin.Forms.Internals;

    public class MenuPageViewModel : BaseViewModel
    {
        private bool _isRefreshing = false;

        public ObservableCollection<HomeMenuItemsGroup> MenuItems { get; set; }

        /// <summary>
        /// Get if the menu items list is refreshing or not.
        /// </summary>
        public bool IsRefreshing
        {
            get { return this._isRefreshing; }
            set { this.SetProperty(ref this._isRefreshing, value); }
        }

        public Command LoadMenuItemsCommand { get; set; }

        public MenuPageViewModel()
        {
            this.Title = "Menu";
            this.MenuItems = new ObservableCollection<HomeMenuItemsGroup>();
            this.LoadMenuItemsCommand = new Command(async () => await this.ExecuteLoadCollectionsCommand());
        }

        async Task ExecuteLoadCollectionsCommand()
        {
            if (this.IsBusy) return;
            this.IsBusy = true;
            this.IsRefreshing = true;
            this.MenuItems.Clear();

            try
            {
                int id = 0;

                var basicCollections = new List<Collection>();
                try
                {
                    basicCollections = await this.CollectionsStore.GetAllUserCollectionsAsync(this.UserId, true);
                }
                catch (Exception e)
                {
                    Log.Warning("LOAD_COLLECTIONS", $"Exception thrown: {e.Message}");
                }

                if (basicCollections.Count == 0)
                {
                    var res = await this.CollectionsStore.CreateCollectionAsync("1", this.UserId);
                    if (res != null)
                    {
                        basicCollections = await this.CollectionsStore.GetAllUserCollectionsAsync(this.UserId, true);
                    }
                }

                var collectionMenuItems = new HomeMenuItemsGroup("Collection", "Collection view");
                basicCollections.ForEach(collection =>
                {
                    collectionMenuItems.Add(new HomeMenuItem
                    {
                        Id = id++,
                        MenuType = MenuItemType.Browse,
                        Title = this.GenerateMenuItemTitle(FontAwesomeIcons.FolderOpen, collection.Name),
                        Arg = collection.Id
                    });
                });

                this.MenuItems.Add(collectionMenuItems);
                this.MenuItems.Add(new HomeMenuItemsGroup("Settings", "Settings") {
                    new HomeMenuItem {
                        Id = id++,
                        MenuType = MenuItemType.Settings,
                        Title = this.GenerateMenuItemTitle(FontAwesomeIcons.FileAlt, "Settings")
                    }, new HomeMenuItem {
                        Id = id++,
                        MenuType = MenuItemType.About,
                        Title = this.GenerateMenuItemTitle(FontAwesomeIcons.MapMarkerQuestion, "About")
                    }, new HomeMenuItem {
                        Id = id++,
                        MenuType = MenuItemType.Logout,
                        Title = this.GenerateMenuItemTitle(FontAwesomeIcons.TimesCircle, "Logout")
                    }
                });
            }
            catch (Exception ex)
            {
                // This may occure when api fail twice to load collection.
                Debug.WriteLine(ex);
            }
            finally
            {
                this.IsBusy = false;
                this.IsRefreshing = false;
            }
        }

        private string GenerateMenuItemTitle(string icon, string title) => icon + " " + title;

        public async Task ExecuteMenuSelectionCommand(HomeMenuItem menuItem)
        {
            if (this.IsBusy) return;
            this.IsBusy = true;
            this.IsRefreshing = true;

            try
            {
                // Navigate to the selected menu voice.
                if (menuItem == null || !(Application.Current.MainPage is MainPage))
                    return;

                this.IsBusy = false;
                this.IsRefreshing = false;
                await ((MainPage)Application.Current.MainPage).NavigateFromMenu(menuItem);
            }
            catch (Exception ex)
            {
                Log.Warning("MENU NAVIGATION", $"Exception: {ex.StackTrace}");
            }
            finally
            {
                this.IsBusy = false;
                this.IsRefreshing = false;
            }
        }
    }
}
