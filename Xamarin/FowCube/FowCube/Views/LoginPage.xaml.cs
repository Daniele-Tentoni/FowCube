namespace FowCube.Views
{
    using FowCube.Authentication;
    using FowCube.Models;
    using System;
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
            this.GoogleLoginButton.Clicked += this.GoogleLoginButton_Clicked;
            this.auth = DependencyService.Get<IAuth>();
        }

        private void GoogleLoginButton_Clicked(object sender, EventArgs e) => this.auth.LoginWithGoogleAuth(this.OnLoginComplete);

        void LoginClicked(object sender, EventArgs e)
        {
            string mail = this.EmailEntry.Text;
            string pass = this.PasswordEntry.Text;

            try
            {
                this.auth.LoginWithEmailPasswordAsync(mail, pass, this.OnLoginComplete);
            }
            catch (Exception ex)
            {
                Log.Warning("LOGIN", $"Authentication failed: {ex.Message}");
                this.ShowError();
            }
        }

        private void OnLoginComplete(User user, string message)
        {
            if (user != null)
            {
                SecureStorage.SetAsync("display_name", string.IsNullOrEmpty(user.DisplayName) ? "null" : user.DisplayName);
                SecureStorage.SetAsync("user_id", string.IsNullOrEmpty(user.Id) ? "null" : user.Id);
                SecureStorage.SetAsync("email", user.Email);
                Device.BeginInvokeOnMainThread(() =>
                {
                    Application.Current.MainPage = new MainPage();
                });
            }
            else this.ShowError(message);
        }

        async private void ShowError(string message = "") => 
            await this.DisplayAlert("Authentication Failed", 
                message != "" ? message : "E-mail or password are incorrect. Try again!", 
                "OK");
    }
}