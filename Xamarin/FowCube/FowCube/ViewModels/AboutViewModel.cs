namespace FowCube.ViewModels
{
    using System;
    using System.Windows.Input;
    using Xamarin.Essentials;
    using Xamarin.Forms;

    public class AboutViewModel : BaseViewModel
    {
        public AboutViewModel()
        {
            this.Title = "About";

            this.OpenWebCommand = new Command(() => Launcher.OpenAsync(new Uri("https://xamarin.com/platform")));
        }

        public ICommand OpenWebCommand { get; }
    }
}