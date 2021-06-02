﻿using System;
using System.Collections.Generic;
using System.Text;
using IROFramework.Core.AppEnvironment;
using IROFramework.Core.AppEnvironment.SettingsDto;
using IROFramework.Web.Services;
using IROFramework.Web.Services.Auth;
using IROFramework.Web.Tools.JwtAuth;
using IROFramework.Web.Tools.JwtAuth.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;

namespace IROFramework.Web.StartupInit
{
    public static class AuthInitExtensions
    {
        public static void AddMyAuth(this IServiceCollection services)
        {
            var jwtAuthOpt = new JwtAuthManagerOptions()
            {
                AccessTokenExpiration = TimeSpan.FromMinutes(Env.GetValue<AuthSettings>().AccessTokenExpirationMinutes),
                RefreshTokenExpiration = TimeSpan.FromMinutes(Env.GetValue<AuthSettings>().RefreshTokenExpirationMinutes),
                Secret = Env.GetValue<AuthSettings>().Secret,
                Audience = Env.ExternalUrl,
                Issuer = Env.ExternalUrl
            };
            services.AddSingleton(jwtAuthOpt);
            services.AddSingleton<IJwtAuthManager, JwtAuthManager>();
            services.AddSingleton<IUserAuthService, UserAuthService>();

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = true;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtAuthOpt.Issuer,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtAuthOpt.Secret)),
                    ValidAudience = jwtAuthOpt.Audience,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(1)
                };
            });


        }

        /// <summary>
        /// Add this before UseAuthentification and you can use cookie param <see cref="CommonConsts.AuthTokenCookieParam"/> for auth.
        /// </summary>
        public static void UseCookieAuthToken(this IApplicationBuilder app)
        {
            app.Use(async (ctx, next) =>
            {
                try
                {
                    if (ctx.Request.Cookies.ContainsKey(CommonConsts.AuthTokenCookieParam))
                    {
                        ctx.Request.Headers[HeaderNames.Authorization] =
                            "Bearer " + ctx.Request.Cookies[CommonConsts.AuthTokenCookieParam];
                    }
                }
                finally
                {
                    await next();
                }
            });
        }

        public static void UseAlwaysRedirectIfNotLogin(this IApplicationBuilder app)
        {
            app.Use(async (ctx, next) =>
            {
                var notLogin = ctx.User.Identity.IsAuthenticated;
                var url = ctx.Request.Path.Value;
                if (notLogin && !url.StartsWith("/" + CommonConsts.ApiPath) && !url.StartsWith("/login"))
                {
                    ctx.Response.Redirect(Env.ExternalUrl + "/login");
                }
                else
                {
                    await next();
                }
            });
        }
    }
}
