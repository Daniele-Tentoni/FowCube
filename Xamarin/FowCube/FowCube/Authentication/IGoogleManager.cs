namespace FowCube.Authentication
{
    using FowCube.Models;
    using System;

    public interface IGoogleManager
    {
        void Login(Action<User, string> onLoginComplete);
        void Logout();
    }
}
