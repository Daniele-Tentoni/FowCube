namespace FowCube.Views
{
    using FowCube.Authentication;
    using System;
    using System.Threading.Tasks;
    using Xamarin.Essentials;
    using Xamarin.Forms;
    using Xamarin.Forms.Internals;
    using Xamarin.Forms.Xaml;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        private readonly IAuth auth;
        public LoginPage()
        {
            this.InitializeComponent();
            this.LoginButton.Clicked += this.LoginClicked;
            this.auth = DependencyService.Get<IAuth>();
        }

        async void LoginClicked(object sender, EventArgs e)
        {
            string mail = this.EmailEntry.Text;
            string pass = this.PasswordEntry.Text;

            if (await this.ExecuteLogin(mail, pass))
            {
                this.LoginExecuted();
            }
        }

        async Task<bool> ExecuteLogin(string mail, string pass)
        {
            try
            {
                var result = await this.auth.LoginWithEmailPasswordAsync(mail, pass);
                return !string.IsNullOrEmpty(result);
            }
            catch (Exception ex)
            {
                Log.Warning("LOGIN", $"Authentication failed: {ex.Message}");
                await this.ShowError();
            }
            return false;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            string authId = this.auth.GetAuthenticatedUid();
            if (!string.IsNullOrEmpty(authId))
            {
                this.LoginExecuted();
            }
        }

        private void LoginExecuted() => Application.Current.MainPage = new MainPage();

        async private Task ShowError() => await this.DisplayAlert("Authentication Failed", "E-mail or password are incorrect. Try again!", "OK");
    }
}