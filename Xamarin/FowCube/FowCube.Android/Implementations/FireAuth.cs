using System;
using Android.Gms.Auth.Api;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Gms.Tasks;
using Android.OS;
using Firebase.Auth;
using FowCube.Authentication;
using FowCube.Droid.Implementations;
using FowCube.Models;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

[assembly: Xamarin.Forms.Dependency(typeof(FireAuth))]
namespace FowCube.Droid.Implementations
{
    public class FireAuth : Java.Lang.Object, IAuth, GoogleApiClient.IConnectionCallbacks, GoogleApiClient.IOnConnectionFailedListener
    {
        private const string TAG = "FIREBASE_LOGIN";
        public Action<User, string> _onLoginComplete;
        public static GoogleApiClient GoogleApiClient { get; set; }
        public static FireAuth Instance { get; private set; }

        public FireAuth()
        {
            Instance = this;
            var gso = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
                .RequestIdToken("738002101933-57crr6hvomm49gasqaj3fa5tnrshsdd7.apps.googleusercontent.com")
                .RequestEmail().Build();
            GoogleApiClient = new GoogleApiClient.Builder(((MainActivity)Forms.Context).ApplicationContext)
                .AddConnectionCallbacks(this)
                .AddOnConnectionFailedListener(this)
                .AddApi(Auth.GOOGLE_SIGN_IN_API, gso)
                .AddScope(new Scope(Scopes.Profile))
                .Build();
        }

        public async void LoginWithEmailPasswordAsync(string email, string password, Action<User, string> onLoginComplete)
        {
            try
            {
                var user = await FirebaseAuth.Instance.SignInWithEmailAndPasswordAsync(email, password);
                var token = await user.User.GetIdTokenAsync(false);
                var logged = new User
                {
                    Id = user.User.Uid,
                    Username = user.AdditionalUserInfo.Username,
                    DisplayName = user.User.DisplayName,
                    Email = user.User.Email,
                    Picture = (user.User.PhotoUrl != null ? $"{ user.User.PhotoUrl}" : $"https://autisticdating.net/imgs/profile-placeholder.jpg")
                };
                onLoginComplete?.Invoke(logged, string.Empty);
            }
            catch (FirebaseAuthInvalidUserException e)
            {
                e.PrintStackTrace();
            }
            catch (Exception e)
            {
                Log.Warning("FIREBASE LOGIN", e.StackTrace);
            }
        }

        public void LoginWithGoogleAuth(Action<User, string> onLoginComplete)
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
                var credentials = GoogleAuthProvider.GetCredential(result.SignInAccount.IdToken, null);
                FirebaseAuth.Instance.SignInWithCredential(credentials).AddOnCompleteListener(new CompleteListener(this._onLoginComplete));
            }
            else
            {
                Log.Warning(TAG, result.Status.ToString());
                var message = result.Status.StatusCode == 10 ? "Developer error." : "An error occured!";
                this._onLoginComplete?.Invoke(null, message);
            }
        }

        public void Logout()
        {
            Auth.GoogleSignInApi.SignOut(GoogleApiClient).SetResultCallback(new ResultCallback<IResult>((obj) =>
            {
                FirebaseAuth.Instance.SignOut();
                Log.Warning(TAG, "Logout.");
            }));
        }

        public User GetAuthenticatedUser()
        {
            if (FirebaseAuth.Instance != null && FirebaseAuth.Instance.CurrentUser != null)
                return new User
                {
                    DisplayName = FirebaseAuth.Instance.CurrentUser.DisplayName,
                    Id = FirebaseAuth.Instance.CurrentUser.Uid
                };
            return null;
        }

        public void OnConnected(Bundle connectionHint) { }
        public void OnConnectionSuspended(int cause) => this._onLoginComplete?.Invoke(null, "Canceled!");
        public void OnConnectionFailed(ConnectionResult result) => this._onLoginComplete?.Invoke(null, result.ErrorMessage);
    }
}