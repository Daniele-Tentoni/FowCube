namespace FowCube.Views
{
    using FowCube.Authentication;
    using FowCube.Models;
    using FowCube.Utils;
    using FowCube.Utils.Strings;
    using System;
    using Xamarin.Essentials;
    using Xamarin.Forms;
    using Xamarin.Forms.Internals;
    using Xamarin.Forms.Xaml;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        private readonly IAuth auth = DependencyService.Get<IAuth>();

        public string LoginSubtitle = string.Format(AppStrings.LoginSubTitle, FontAwesomeIcons.Heart);

        public LoginPage()
        {
            this.InitializeComponent();
            this.LoginButton.Clicked += this.LoginClicked;
            this.GoogleLoginButton.Clicked += this.GoogleLoginButton_Clicked;
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
                // Add the user to SecureStorage and Realm.
                SecureStorage.SetAsync("user_id", string.IsNullOrEmpty(user.Id) ? "null" : user.Id);
                Device.BeginInvokeOnMainThread(() =>
                {
                    Application.Current.MainPage = new MainPage();
                });
            }
            else this.ShowError(message);
        }

        /// <summary>
        /// Show authentication errors to user.
        /// </summary>
        /// <param name="message">Message to show.</param>
        async private void ShowError(string message = "") => 
            await this.DisplayAlert("Authentication Failed", 
                message != "" ? message : "E-mail or password are incorrect. Try again!", 
                "OK");
    }
}