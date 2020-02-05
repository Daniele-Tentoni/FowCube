namespace FowCube.Authentication
{
    using FowCube.Models;
    using System;

    public interface IAuth
    {
        /// <summary>
        /// Return the User Authenticated.
        /// </summary>
        /// <returns>Current firebase user.</returns>
        User GetAuthenticatedUser();

        /// <summary>
        /// Execute the login to firebase with email and password provider.
        /// </summary>
        /// <param name="email">User email.</param>
        /// <param name="password">User password.</param>
        /// <returns>User token.</returns>
        void LoginWithEmailPasswordAsync(string email, string password, Action<User, string> onLoginComplete);

        /// <summary>
        /// Execute the login to firebase with google auth provider.
        /// </summary>
        /// <param name="onLoginComplete">Callback when login is completed.</param>
        void LoginWithGoogleAuth(Action<User, string> onLoginComplete);

        /// <summary>
        /// Execute the logout from all auth providers.
        /// </summary>
        void Logout();
    }
}
