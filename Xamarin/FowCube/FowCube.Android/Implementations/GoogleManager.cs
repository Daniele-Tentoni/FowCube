namespace FowCube.Droid.Implementations
{
    using Android.Gms.Auth.Api;
    using Android.Gms.Auth.Api.SignIn;
    using Android.Gms.Common;
    using Android.Gms.Common.Apis;
    using Android.OS;
    using FowCube.Authentication;
    using FowCube.Models;
    using System;
    using Xamarin.Forms;

    public class GoogleManager : Java.Lang.Object, IGoogleManager, GoogleApiClient.IConnectionCallbacks, GoogleApiClient.IOnConnectionFailedListener
    {
        public Action<User, string> _onLoginComplete;
        public static GoogleApiClient GoogleApiClient { get; set; }
        public static GoogleManager Instance { get; private set; }

        public GoogleManager()
        {
            Instance = this;
            var gso = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
                .RequestEmail().Build();
            GoogleApiClient = new GoogleApiClient.Builder(((MainActivity)Forms.Context).ApplicationContext)
                .AddConnectionCallbacks(this)
                .AddOnConnectionFailedListener(this)
                .AddApi(Auth.GOOGLE_SIGN_IN_API, gso)
                .AddScope(new Scope(Scopes.Profile))
                .Build();
        }

        public void Login(Action<User, string> onLoginComplete)
        {
            this._onLoginComplete = onLoginComplete;
            var signInIntent = Auth.GoogleSignInApi.GetSignInIntent(GoogleApiClient);
            ((MainActivity)Forms.Context).StartActivityForResult(signInIntent, 1);
            GoogleApiClient.Connect();
        }

        public void OnAuthCompleted(GoogleSignInResult result)
        {
            if (result.IsSuccess)
            {
                var ac = result.SignInAccount;
                this._onLoginComplete?.Invoke(new User
                {
                    Id = ac.Id,
                    Username = ac.GivenName,
                    DisplayName = ac.DisplayName,
                    Email = ac.Email,
                    Picture = new Uri((ac.PhotoUrl != null ? $"{ac.PhotoUrl}" : $"https://autisticdating.net/imgs/profile-placeholder.jpg"))
                }, string.Empty);
            }
            else
            {
                this._onLoginComplete?.Invoke(null, "An error occured!");
            }
        }

        public void Logout() => GoogleApiClient.Disconnect();
        public void OnConnected(Bundle connectionHint) { }
        public void OnConnectionSuspended(int cause) => this._onLoginComplete?.Invoke(null, "Canceled!");
        public void OnConnectionFailed(ConnectionResult result) => this._onLoginComplete?.Invoke(null, result.ErrorMessage);
    }
}