using System;
using System.Threading.Tasks;
using IRO.Mvc.CoolSwagger;
using IROFramework.Core.AppEnvironment;
using IROFramework.Core.Consts;
using IROFramework.Core.Models;
using IROFramework.Web.Dto.AuthDto;
using IROFramework.Web.Services.Auth;
using IROFramework.Web.Tools.JwtAuth.Data;
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
        /// Default login page. Used only in tests.
        /// </summary>
        [HttpGet("oauthCallback")]
        public LoginResponse OAuthCallback([FromQuery] string accessToken, [FromQuery] string refreshToken)
        {
            var authRes = new AuthResult()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
            return this.MakeLoginResponse(authRes);
        }

        /// <summary>
        /// Works only in debug mode.
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpGet("makeMeAdmin")]
        public async Task<LoginResponse> MakeMeAdmin()
        {
            if (!Env.IsDebug)
            {
                throw new Exception("Works only in debug mode.");
            }
            var me = await this.GetCurrentUser();
            me = await _userAuthService.ChangeRole(me, UserRoles.Admin);
            var authRes = _userAuthService.GenerateToken(me);
            return this.MakeLoginResponse(authRes);
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
            this.DeleteAuthCookie();
        }

        [HttpPost("refreshToken")]
        public async Task<LoginResponse> RefreshToken(RefreshTokenRequest dto)
        {
            var authRes = await _userAuthService.RefreshToken(dto.AccessToken, dto.RefreshToken);
            return this.MakeLoginResponse(authRes);
        }

        [HttpPost("loginByNickname")]
        public async Task<LoginResponse> LoginByNickname(LoginByNicknameRequest dto)
        {
            var authRes = await _userAuthService.Login(dto.Nickname, dto.Password);
            return this.MakeLoginResponse(authRes);
        }

        [HttpPost("registerByNickname")]
        public async Task<LoginResponse> RegisterByNickname(LoginByNicknameRequest dto)
        {
            var authRes = await _userAuthService.Register(dto.Nickname, dto.Password);
            return this.MakeLoginResponse(authRes);
        }

    }
}