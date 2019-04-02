using System.Threading.Tasks;

namespace Rinsen.IdentityProvider
{
    public interface ILoginService
    {
        Task<LoginResult> LoginAsync(string email, string password, string host, bool rememberMe);
        Task LogoutAsync();
    }
}
