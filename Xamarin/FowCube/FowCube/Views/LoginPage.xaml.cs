namespace FowCube.Views
{
    using FowCube.Authentication;
    using System;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        private readonly IAuth auth;
        public LoginPage()
        {
            this.InitializeComponent();
            this.auth = DependencyService.Get<IAuth>();
            this.LoginButton.Clicked += this.LoginClicked;
        }

        async void LoginClicked(object sender, EventArgs e)
        {
            try
            {
                string Token = await this.auth.LoginWithEmailPasswordAsync(this.EmailEntry.Text, this.PasswordEntry.Text);
                if (Token != "")
                {
                    App.Current.MainPage = new MainPage();
                }
                else
                {
                    this.ShowError();
                }
            }
            catch(Exception ex)
            {
                await this.DisplayAlert("Authentication Failed", $"{ex.Message}: {this.auth == null}", "OK");
            }
        }

        async private void ShowError() => await this.DisplayAlert("Authentication Failed", "E-mail or password are incorrect. Try again!", "OK");
    }
}