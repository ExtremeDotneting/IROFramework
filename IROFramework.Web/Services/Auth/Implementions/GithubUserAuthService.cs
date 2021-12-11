using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using IRO.Common.Text;
using IROFramework.Core.AppEnvironment.SettingsDto;
using IROFramework.Core.Models;
using IROFramework.Core.Tools.AbstractDatabase;
using IROFramework.Web.Tools.JwtAuth.Data;
using Octokit;

namespace IROFramework.Web.Services.Auth
{
    class GithubUserAuthService : IGithubUserAuthService
    {
        readonly IUserAuthService _userAuthService;
        readonly IDatabaseSet<UserModel, Guid> _dbSet;
        readonly GithubAuthSettings _githubAuthSettings;

        public GithubUserAuthService(IUserAuthService userAuthService, IAbstractDatabase db, GithubAuthSettings githubAuthSettings)
        {
            _userAuthService = userAuthService;
            _githubAuthSettings = githubAuthSettings;
            _dbSet = db.GetDbSet<UserModel, Guid>();
        }

        public async Task<AuthResult> LoginOrRegisterUsingOauthCode(string oauthCode)
        {
            var githubClient = ResolveClient();

            var oauthReq = new OauthTokenRequest(
                _githubAuthSettings.ClientId,
                _githubAuthSettings.ClientSecret,
                oauthCode
            );
            var tokenResp = await githubClient.Oauth.CreateAccessToken(oauthReq);
            githubClient.Credentials = new Credentials(tokenResp.AccessToken);
            var githubUser = await githubClient.User.Current();

            var user = await _dbSet.TryGetByPropertyAsync(r => r.Github_UserId, githubUser.Id);
            AuthResult authResult;
            if (user == null)
            {
                authResult = await _userAuthService.Register(
                    githubUser.Login, 
                    TextExtensions.Generate(10)
                    );
                user = await _userAuthService.GetByToken(authResult.AccessToken);
            }
            else
            {
                authResult = await _userAuthService.LoginByUserId(user.Id);
            }

            user.Github_UserId = githubUser.Id;
            user.Github_Login = githubUser.Login;
            user.Github_AccessToken = tokenResp.AccessToken;
            await _dbSet.UpdateAsync(user);

            return authResult;
        }


        GitHubClient ResolveClient()
        {
            var githubClient = new GitHubClient(new ProductHeaderValue(_githubAuthSettings.AppName));
            return githubClient;
        }
    }
}
