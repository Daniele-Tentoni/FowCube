using System;
using System.Threading.Tasks;
using Firebase.Auth;
using FowCube.Authentication;
using FowCube.Droid.Implementations;
using FowCube.Models;
using Xamarin.Forms.Internals;

[assembly: Xamarin.Forms.Dependency(typeof(FireAuth))]
namespace FowCube.Droid.Implementations
{
    public class FireAuth : IAuth
    {
        public string email;
        public string password;

        public FireAuth()
        {
            this.email = this.password = "";
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
                    Email = user.User.Email,
                    Picture = new Uri((user.User.PhotoUrl != null ? $"{ user.User.PhotoUrl}" : $"https://autisticdating.net/imgs/profile-placeholder.jpg"))
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

        public void Logout() => FirebaseAuth.Instance.SignOut();

        public string GetAuthenticatedUid()
        {
            if (FirebaseAuth.Instance != null && FirebaseAuth.Instance.CurrentUser != null)
                return FirebaseAuth.Instance.CurrentUser.Uid;
            return string.Empty;
        }

        public string GetAuthenticatedDisplayName() => FirebaseAuth.Instance.CurrentUser.DisplayName;
    }
}