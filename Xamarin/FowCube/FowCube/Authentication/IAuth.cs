namespace FowCube.Authentication
{
    using System.Threading.Tasks;

    public interface IAuth
    {
        string GetAuthenticatedUid();
        Task<string> LoginWithEmailPasswordAsync(string email, string password);
    }
}
