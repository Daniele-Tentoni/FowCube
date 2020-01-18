namespace FowCube.Authentication
{
    using FowCube.Models;
    using System;
    using System.Threading.Tasks;

    public interface IAuth
    {
        /// <summary>
        /// Return the User Identifier of Current User.
        /// </summary>
        /// <returns></returns>
        string GetAuthenticatedUid();

        /// <summary>
        /// Return the Display Name of Current User.
        /// </summary>
        /// <returns></returns>
        string GetAuthenticatedDisplayName();

        /// <summary>
        /// Execute the login to firebase authentication provider.
        /// </summary>
        /// <param name="email">User email.</param>
        /// <param name="password">User password.</param>
        /// <returns>User token.</returns>
        void LoginWithEmailPasswordAsync(string email, string password, Action<User, string> onLoginComplete);

        /// <summary>
        /// Execute the logout from firebase.
        /// </summary>
        void Logout();
    }
}
