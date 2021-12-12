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
        readonly IGithubUserAuthService _githubUserAuthService;
        readonly GithubAuthSettings _githubAuthSettings;
        readonly GlobalSettings _globalSettings;

        public GithubUserAuthController(IGithubUserAuthService githubUserAuthService, GithubAuthSettings githubAuthSettings, GlobalSettings globalSettings)
        {
            _githubUserAuthService = githubUserAuthService;
            _githubAuthSettings = githubAuthSettings;
            _globalSettings = globalSettings;
        }


        [HttpGet("login")]
        public async Task<IActionResult> Login()
        {
            var redirectUri = $"{_globalSettings.ExternalUrl}/{CommonConsts.ApiPath}/auth/github/callback";
            var url = _githubUserAuthService.GetLoginUrl(redirectUri);
            return new RedirectResult(url);
        }

        [HttpGet("callback")]
        public async Task<LoginResponse> Callback([FromQuery] string code)
        {
            var authRes = await _githubUserAuthService.LoginOrRegisterUsingOauthCode(code);
            return this.MakeLoginResponse(authRes);
        }


    }
}