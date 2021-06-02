using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using IROFramework.Core.Models;
using IROFramework.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;

// ReSharper disable once CheckNamespace
namespace IROFramework.Web.Services.Auth
{
    public static class CtxAuthExtensions
    {
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
