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

        public ICommand OpenXamarinCommand { get; } = new Command(() => Launcher.OpenAsync(new Uri("https://xamarin.com/platform")));

        public ICommand OpenGitHubCommand { get; } = new Command(() => Launcher.OpenAsync(new Uri("https://github.com/Daniele-Tentoni/FowCube")));

        public ICommand OpenGitHubIssuesCommand { get; } = new Command(() => Launcher.OpenAsync(new Uri("https://github.com/Daniele-Tentoni/FowCube/issues")));

        public string AppName => AppInfo.Name;

        public string AppVersion => AppInfo.Version.ToString();
    }
}