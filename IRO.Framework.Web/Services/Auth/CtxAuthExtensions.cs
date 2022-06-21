using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using IROFramework.Core.AppEnvironment;
using IROFramework.Core.Models;
using IROFramework.Web.Dto.AuthDto;
using IROFramework.Web.Tools.JwtAuth.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;

// ReSharper disable once CheckNamespace
namespace IROFramework.Web.Services.Auth
{
    public static class CtxAuthExtensions
    {
        public static LoginResponse MakeLoginResponse(this ControllerBase controller, AuthResult authResult)
        {
            DeleteAuthCookie(controller);
            controller.Response.Cookies.Append(CommonConsts.AuthTokenCookieParam, authResult.AccessToken);
            return new LoginResponse()
            {
                AccessToken = authResult.AccessToken,
                RefreshToken = authResult.RefreshToken
            };
        }

        public static void DeleteAuthCookie(this ControllerBase controller)
        {
            if (controller.Request.Cookies.ContainsKey(CommonConsts.AuthTokenCookieParam))
            {
                controller.Response.Cookies.Delete(CommonConsts.AuthTokenCookieParam);
            }
        }

        public static async Task<UserModel> GetCurrentUser(this HttpContext ctx)
        {
            var token = GetAuthToken(ctx);
            var userAuthService = ctx.RequestServices.GetRequiredService<IUserAuthService>();
            try
            {
                return await userAuthService.GetByToken(token);
            }
            catch (Exception ex)
            {
                throw new UnauthorizedAccessException("User is unauthorized.", ex);
            }
        }

        public static async Task<UserModel> GetCurrentUser(this ControllerBase controller)
        {
            return await controller.HttpContext.GetCurrentUser();
        }

        static string GetAuthToken(HttpContext ctx)
        {
            var authorization = ctx.Request.Headers[HeaderNames.Authorization];
            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                var token = headerValue.Parameter;
                return token;
            }
            throw new Exception("Auth token not found in header.");
        }
    }
}
