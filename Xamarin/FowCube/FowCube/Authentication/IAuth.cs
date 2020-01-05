namespace FowCube.Authentication
{
    using System.Threading.Tasks;

    public interface IAuth
    {
        Task<string> LoginWithEmailPasswordAsync(string email, string password);
    }
}
