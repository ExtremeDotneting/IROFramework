using System;
using System.Threading.Tasks;
using IRO.Mvc.CoolSwagger;
using IROFramework.Core.AppEnvironment;
using IROFramework.Core.AppEnvironment.SettingsDto;
using IROFramework.Core.Consts;
using IROFramework.Core.Models;
using IROFramework.Core.Tools;
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

        public GithubUserAuthController(IGithubUserAuthService githubUserAuthService, GithubAuthSettings githubAuthSettings, GlobalSettings globalSettings)
        {
            _githubUserAuthService = githubUserAuthService;
            _githubAuthSettings = githubAuthSettings;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="redirectUrl">If null </param>
        /// <returns></returns>
        [HttpGet("login")]
        public async Task<IActionResult> Login([FromQuery] string redirectUrl)
        {
            if (string.IsNullOrWhiteSpace(redirectUrl))
            {
                redirectUrl = $"{UrlExtensions.GetOwnApiUrl()}/auth/oauthCallback";
            }
            var githubRedirectUrl = $"{UrlExtensions.GetOwnApiUrl()}/auth/github/callback?redirectUrl={redirectUrl.UrlEncode()}";
            var url = _githubUserAuthService.GetLoginUrl(githubRedirectUrl);
            return new RedirectResult(url);
        }

        [HttpGet("callback")]
        public async Task<RedirectResult> Callback([FromQuery] string code, [FromQuery] string redirectUrl)
        {
            var authRes = await _githubUserAuthService.LoginOrRegisterUsingOauthCode(code);
            redirectUrl = redirectUrl.RemoveEndingSlash();

            var startSymbol = "?";
            if (redirectUrl.Contains("?"))
            {
                startSymbol = "&";
            }
            redirectUrl += $"{startSymbol}accessToken={authRes.AccessToken.UrlEncode()}&refreshToken={authRes.RefreshToken.UrlEncode()}";
            return new RedirectResult(redirectUrl);
        }


    }
}