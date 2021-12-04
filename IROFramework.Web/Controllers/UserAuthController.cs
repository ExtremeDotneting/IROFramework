using System;
using System.Threading.Tasks;
using IRO.Mvc.CoolSwagger;
using IROFramework.Core.AppEnvironment;
using IROFramework.Core.Consts;
using IROFramework.Core.Models;
using IROFramework.Web.Dto.AuthDto;
using IROFramework.Web.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IROFramework.Web.Controllers
{
    [ApiController]
    [SwaggerTagName("UserAuth")]
    [Route(CommonConsts.ApiPath + "/auth")]
    public class UserAuthController : ControllerBase
    {
        readonly IUserAuthService _userAuthService;

        public UserAuthController(IUserAuthService userAuthService)
        {
            _userAuthService = userAuthService;
        }

        /// <summary>
        /// Works only in debug mode.
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpGet("makeMeAdmin")]
        public async Task<LoginResponseResponse> MakeMeAdmin()
        {
            if (!Env.IsDebug)
            {
                throw new Exception("Works only in debug mode.");
            }
            var me = await this.GetCurrentUser();
            me = await _userAuthService.ChangeRole(me, UserRoles.Admin);

            DeleteOldCookie();
            var authRes = _userAuthService.GenerateToken(me);
            Response.Cookies.Append(CommonConsts.AuthTokenCookieParam, authRes.AccessToken);
            return new LoginResponseResponse()
            {
                AccessToken = authRes.AccessToken,
                RefreshToken = authRes.RefreshToken
            };
        }


        [Authorize]
        [HttpGet("getMe")]
        public async Task<UserModel> GetMe()
        {
            return await this.GetCurrentUser();
        }

        [HttpPost("logout")]
        public void Logout()
        {
            DeleteOldCookie();
        }

        [HttpPost("refreshToken")]
        public async Task<LoginResponseResponse> RefreshToken(RefreshTokenRequest dto)
        {
            DeleteOldCookie();
            var authRes = await _userAuthService.RefreshToken(dto.AccessToken, dto.RefreshToken);
            Response.Cookies.Append(CommonConsts.AuthTokenCookieParam, authRes.AccessToken);
            return new LoginResponseResponse()
            {
                AccessToken = authRes.AccessToken,
                RefreshToken = authRes.RefreshToken
            };
        }

        [HttpPost("loginByNickname")]
        public async Task<LoginResponseResponse> LoginByNickname(LoginByNicknameRequest dto)
        {
            DeleteOldCookie();
            var authRes = await _userAuthService.Login(dto.Nickname, dto.Password);
            Response.Cookies.Append(CommonConsts.AuthTokenCookieParam, authRes.AccessToken);
            return new LoginResponseResponse()
            {
                AccessToken = authRes.AccessToken,
                RefreshToken = authRes.RefreshToken
            };
        }

        [HttpPost("registerByNickname")]
        public async Task<LoginResponseResponse> RegisterByNickname(LoginByNicknameRequest dto)
        {
            DeleteOldCookie();
            var authRes = await _userAuthService.Register(dto.Nickname, dto.Password);
            Response.Cookies.Append(CommonConsts.AuthTokenCookieParam, authRes.AccessToken);
            return new LoginResponseResponse()
            {
                AccessToken = authRes.AccessToken,
                RefreshToken = authRes.RefreshToken
            };
        }

        void DeleteOldCookie()
        {
            if (Request.Cookies.ContainsKey(CommonConsts.AuthTokenCookieParam))
            {
                Response.Cookies.Delete(CommonConsts.AuthTokenCookieParam);
            }
        }
    }
}