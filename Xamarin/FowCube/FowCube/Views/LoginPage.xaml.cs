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
            this.auth = DependencyService.Get<IAuth>();
            string mail = SecureStorage.GetAsync("user_mail").Result;
            string pass = SecureStorage.GetAsync("user_pass").Result;
            if (string.IsNullOrEmpty(mail) || string.IsNullOrEmpty(pass))
            {
                this.InitializeComponent();
                this.LoginButton.Clicked += this.LoginClicked;
            }
            else
            {
                if (this.ExecuteLogin(mail, pass).Result)
                {
                    _ = this.LoginExecuted(mail, pass);
                }
            }
        }

        async void LoginClicked(object sender, EventArgs e)
        {
            string mail = this.EmailEntry.Text;
            string pass = this.PasswordEntry.Text;

            if (await this.ExecuteLogin(mail, pass))
            {
                await this.LoginExecuted(mail, pass);
            }
        }

        async Task<bool> ExecuteLogin(string mail, string pass)
        {
            try
            {
                return !string.IsNullOrEmpty(await this.auth.LoginWithEmailPasswordAsync(mail, pass));
            }
            catch (Exception ex)
            {
                Log.Warning("LOGIN", $"Authentication failed: {ex.Message}");
                await this.ShowError();
            }
            return false;
        }

        async Task LoginExecuted(string mail, string pass)
        {
            await SecureStorage.SetAsync("user_mail", mail);
            await SecureStorage.SetAsync("user_pass", pass);
            await SecureStorage.SetAsync("login_id", this.auth.GetAuthenticatedUid());
            Application.Current.MainPage = new MainPage();
        }

        async private Task ShowError() => await this.DisplayAlert("Authentication Failed", "E-mail or password are incorrect. Try again!", "OK");
    }
}