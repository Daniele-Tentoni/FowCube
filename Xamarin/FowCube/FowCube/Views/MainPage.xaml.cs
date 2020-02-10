﻿namespace FowCube.Views
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using Xamarin.Forms;
    using FowCube.Authentication;
    using FowCube.Models.HomeMenuItems;

    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : MasterDetailPage
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.MasterBehavior = MasterBehavior.Popover;
        }

        public Dictionary<int, NavigationPage> MenuPages { get; } = new Dictionary<int, NavigationPage>();

        public async Task NavigateFromMenu(HomeMenuItem item)
        {
            // Add the page to the Menu Pages list, that pair Id -> Navigation Page.
            if (!this.MenuPages.ContainsKey(item.Id))
            {
                switch (item.MenuType)
                {
                    case MenuItemType.Browse:
                        this.MenuPages.Add(item.Id, new NavigationPage(new ItemsPage(item.Arg)));
                        break;
                    case MenuItemType.About:
                        this.MenuPages.Add(item.Id, new NavigationPage(new AboutPage()));
                        break;
                    case MenuItemType.Settings:
                        this.MenuPages.Add(item.Id, new NavigationPage(new AboutPage()));
                        break;
                    case MenuItemType.Login:
                        this.MenuPages.Add(item.Id, new NavigationPage(new LoginPage()));
                        break;
                    case MenuItemType.SignOut:
                        this.MenuPages.Add(item.Id, new NavigationPage(new LoginPage()));
                        break;
                }
            }

            if(item.MenuType == MenuItemType.SignOut)
            {
                DependencyService.Get<IAuth>().Logout();
                Application.Current.MainPage = new LoginPage();
                return;
            }

            var newPage = this.MenuPages[item.Id];

            if (newPage != null && this.Detail != newPage)
            {
                this.Detail = newPage;

                if (Device.RuntimePlatform == Device.Android)
                    await Task.Delay(100);

                this.IsPresented = false;
            }
        }
    }
}