﻿using System;
using System.Threading.Tasks;
using Firebase.Auth;
using FowCube.Authentication;
using FowCube.Droid.Implementations;
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

        public async Task<string> LoginWithEmailPasswordAsync(string email, string password)
        {
            try
            {
                var user = await FirebaseAuth.Instance.SignInWithEmailAndPasswordAsync(email, password);
                var token = await user.User.GetIdTokenAsync(false);
                return token.Token;
            }
            catch (FirebaseAuthInvalidUserException e)
            {
                e.PrintStackTrace();
                return "";
            }
            catch (Exception e)
            {
                Log.Warning("FIREBASE LOGIN", e.StackTrace);
                return "";
            }
        }

        public string GetAuthenticatedUid() => FirebaseAuth.Instance.CurrentUser.Uid;
    }
}