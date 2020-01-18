namespace FowCube.Droid.Implementations
{
    using Android.Gms.Auth.Api;
    using Android.Gms.Auth.Api.SignIn;
    using Android.Gms.Common;
    using Android.Gms.Common.Apis;
    using Android.OS;
    using Android.Util;
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
                //.RequestIdToken("738002101933-6fk99vgvvrsth8fsret73bq9o5tfj0k7.apps.googleusercontent.com")
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
                var accountt = result.SignInAccount;
                this._onLoginComplete?.Invoke(new User
                {
                    Username = accountt.DisplayName,
                    Id = accountt.Id,
                    Email = accountt.Email,
                    Picture = new Uri((accountt.PhotoUrl != null ? $"{accountt.PhotoUrl}" : $"https://autisticdating.net/imgs/profile-placeholder.jpg"))
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