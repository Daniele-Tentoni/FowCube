namespace FowCube.ViewModels
{
    using FowCube.Models.Collection;
    using FowCube.Models.HomeMenuItems;
    using FowCube.Utils;
    using FowCube.Utils.Strings;
    using FowCube.Views;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using Xamarin.Forms;
    using Xamarin.Forms.Internals;

    public class MenuPageViewModel : BaseViewModel
    {
        private ObservableCollection<HomeMenuItemsGroup> _menuItems;
        public ObservableCollection<HomeMenuItemsGroup> MenuItems
        {
            get { return this._menuItems; }
            set { this.SetProperty(ref this._menuItems, value, nameof(this._menuItems)); }
        }

        private bool _isRefreshing = false;
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

            MessagingCenter.Subscribe<ItemsViewModel, Collection>(this, "Rename", async (obj, item) =>
            {
                this.MenuItems.First(f => f.Title == AppStrings.MenuTitleCollection).First(f => f.Arg == item.Id).Title = item.Name;
                await Device.InvokeOnMainThreadAsync(() => Application.Current.MainPage.DisplayAlert("Rename", $"Renamed to {item.Name}.", "Ok"));
            });
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
                    basicCollections = await App.Database.GetCollectionsByUserAsync("11");
                }
                catch (Exception e)
                {
                    // Apis mustn't load twice the collection. It means that user haven't collections.
                    Log.Warning("LOAD_COLLECTIONS", string.Format(AppStrings.ExceptionMessage, e.Message));
                }

                /*if (basicCollections.Count == 0)
                {
                    var res = await this.CollectionsStore.CreateCollectionAsync("1", this.UserId);
                    if (res != null)
                    {
                        basicCollections = await this.CollectionsStore.GetAllUserCollectionsAsync(this.UserId, true);
                    }
                }*/

                var collectionMenuItems = new HomeMenuItemsGroup(AppStrings.MenuTitleCollection, AppStrings.MenuSubTitleCollection);
                basicCollections.ForEach(collection =>
                {
                    collectionMenuItems.Add(new HomeMenuItem
                    {
                        Id = id++,
                        MenuType = MenuItemType.Browse,
                        Title = this.GenerateMenuItemTitle(FontAwesomeIcons.FolderOpen, collection.Name),
                        Arg = collection.Id,
                        IsInCloud = !string.IsNullOrEmpty(collection.FirebaseId)
                    });
                });

                this.MenuItems.Add(collectionMenuItems);
                this.MenuItems.Add(new HomeMenuItemsGroup(AppStrings.MenuTitleSettings, AppStrings.MenuSubTitleSettings) {
                    new HomeMenuItem {
                        Id = id++,
                        MenuType = MenuItemType.Settings,
                        Title = this.GenerateMenuItemTitle(FontAwesomeIcons.FileAlt, AppStrings.MenuTitleSettings)
                    }, new HomeMenuItem {
                        Id = id++,
                        MenuType = MenuItemType.About,
                        Title = this.GenerateMenuItemTitle(FontAwesomeIcons.MapMarkerQuestion, AppStrings.MenuTitleAbout)
                    }, new HomeMenuItem {
                        Id = id++,
                        MenuType = MenuItemType.SignOut,
                        Title = this.GenerateMenuItemTitle(FontAwesomeIcons.TimesCircle, AppStrings.MenuTitleSignOut)
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
