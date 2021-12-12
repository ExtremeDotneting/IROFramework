using System;
using System.Threading.Tasks;
using IROFramework.Web.Tools.JwtAuth.Data;
using Octokit;

namespace IROFramework.Web.Services.Auth
{
    public interface IGithubUserAuthService
    {
        Task<AuthResult> LoginOrRegisterUsingOauthCode(string oauthCode);

        Task<GitHubClient> GetUserApiClient(Guid userId);

        string GetLoginUrl(string redirectUrl);
    }
}