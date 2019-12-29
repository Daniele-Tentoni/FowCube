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
        }

        public ICommand OpenWebCommand { get; } = new Command(() => Launcher.OpenAsync(new Uri("https://xamarin.com/platform")));
    }
}