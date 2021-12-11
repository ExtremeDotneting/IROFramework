using System;
using System.Threading.Tasks;
using IRO.Mvc.CoolSwagger;
using IROFramework.Core.AppEnvironment;
using IROFramework.Core.AppEnvironment.SettingsDto;
using IROFramework.Core.Consts;
using IROFramework.Core.Models;
using IROFramework.Web.Dto.AuthDto;
using IROFramework.Web.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Octokit;

namespace IROFramework.Web.Controllers
{
    [ApiController]
    [Route(CommonConsts.ApiPath + "/auth/github")]
    public class GithubUserAuthController : ControllerBase
    {
        readonly IUserAuthService _userAuthService;
        readonly GithubAuthSettings _githubAuthSettings;
        readonly GlobalSettings _globalSettings;

        public GithubUserAuthController(IUserAuthService userAuthService, GithubAuthSettings githubAuthSettings, GlobalSettings globalSettings)
        {
            _userAuthService = userAuthService;
            _githubAuthSettings = githubAuthSettings;
            _globalSettings = globalSettings;
        }


        [HttpGet("login")]
        public async Task<IActionResult> Login()
        {
            var redirectUri = $"{_globalSettings.ExternalUrl}/{CommonConsts.ApiPath}/auth/github/callback"; 
            var authUrl = $"https://github.com/login/oauth/authorize"
                              + $"?client_id={_githubAuthSettings.ClientId}&redirect_uri={redirectUri}";
            return new RedirectResult(authUrl);
        }

        [HttpGet("callback")]
        public async Task Callback([FromQuery]string code)
        {
            var github = new GitHubClient(new ProductHeaderValue(_githubAuthSettings.AppName));
            var oauthReq = new OauthTokenRequest(
                _githubAuthSettings.ClientId,
                _githubAuthSettings.ClientSecret,
                code
            );
            var tokenResp=await github.Oauth.CreateAccessToken(oauthReq);
            tokenResp.AccessToken;
        }

        
    }
}