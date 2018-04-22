using System.Threading.Tasks;

namespace Rinsen.IdentityProvider.ExternalApplications
{
    public interface ITokenStorage
    {
        Task CreateAsync(Token token);
        Task<Token> GetAndDeleteAsync(string tokenId);
    }
}
