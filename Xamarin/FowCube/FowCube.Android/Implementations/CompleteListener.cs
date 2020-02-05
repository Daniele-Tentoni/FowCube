using System;
using Android.Gms.Tasks;
using Firebase.Auth;
using FowCube.Models;

namespace FowCube.Droid.Implementations
{
    public class CompleteListener : Java.Lang.Object, IOnCompleteListener
    {
        public Action<User, string> _onLoginComplete;

        public CompleteListener(Action<User, string> onLoginComplete)
        {
            this._onLoginComplete = onLoginComplete;
        }

        public void OnComplete(Task task)
        {
            if (!task.IsSuccessful) this._onLoginComplete?.Invoke(null, "Fuckin error.");
            var ac = FirebaseAuth.Instance.CurrentUser;
            this._onLoginComplete?.Invoke(new User
            {
                Id = ac.Uid,
                DisplayName = ac.DisplayName,
                Email = ac.Email,
                Picture = new Uri(ac.PhotoUrl != null ? $"{ac.PhotoUrl}" : $"https://autisticdating.net/imgs/profile-placeholder.jpg")
            }, string.Empty);
        }
    }
}
