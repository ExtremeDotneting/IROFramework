using System.Threading.Tasks;
using IROFramework.Core.Models;
using IROFramework.Web.Models;
using IROFramework.Web.Tools.JwtAuth.Data;

namespace IROFramework.Web.Services.Auth
{
    public interface IUserAuthService
    {
        Task<AuthResult> RefreshToken(string accessToken, string refreshToken);
        Task<AuthResult> Login(string nick, string pass);
        Task<AuthResult> Register(string nick, string pass);
        Task<UserModel> ChangeRole(UserModel user, string role);
        Task<UserModel> GetByToken(string accessToken);
        AuthResult GenerateToken(UserModel userModel);
    }
}