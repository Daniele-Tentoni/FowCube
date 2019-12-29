namespace FowCube.Views
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using Xamarin.Forms;

    using FowCube.Models;

    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : MasterDetailPage
    {
        public MainPage()
        {
            this.InitializeComponent();

            this.MasterBehavior = MasterBehavior.Popover;

            this.MenuPages.Add((int)MenuItemType.Browse, (NavigationPage)this.Detail);
        }

        public Dictionary<int, NavigationPage> MenuPages { get; } = new Dictionary<int, NavigationPage>();

        public async Task NavigateFromMenu(int id)
        {
            if (!this.MenuPages.ContainsKey(id))
            {
                switch (id)
                {
                    case (int)MenuItemType.Browse:
                        this.MenuPages.Add(id, new NavigationPage(new ItemsPage()));
                        break;
                    case (int)MenuItemType.About:
                        this.MenuPages.Add(id, new NavigationPage(new AboutPage()));
                        break;
                }
            }

            var newPage = this.MenuPages[id];

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