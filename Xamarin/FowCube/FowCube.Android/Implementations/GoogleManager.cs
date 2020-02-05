namespace FowCube.Droid.Implementations
{
    using Android.Gms.Auth.Api;
    using Android.Gms.Auth.Api.SignIn;
    using Android.Gms.Common;
    using Android.Gms.Common.Apis;
    using Android.OS;
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
            
        }

        public void Login(Action<User, string> onLoginComplete)
        {
            
        }

        

        public void Logout() => 
        
    }
}