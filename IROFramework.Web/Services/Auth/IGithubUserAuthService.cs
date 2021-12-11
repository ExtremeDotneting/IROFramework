using System.Threading.Tasks;
using IROFramework.Web.Tools.JwtAuth.Data;

namespace IROFramework.Web.Services.Auth
{
    public interface IGithubUserAuthService
    {
        Task<AuthResult> LoginOrRegisterUsingOauthCode(string oauthCode);
    }
}