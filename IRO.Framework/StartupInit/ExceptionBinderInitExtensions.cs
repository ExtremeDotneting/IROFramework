using System;
using IRO.Mvc.MvcExceptionHandler;
using IRO.Mvc.MvcExceptionHandler.Services;
using IROFramework.Core.AppEnvironment;
using Microsoft.AspNetCore.Builder;
using Newtonsoft.Json;

namespace IROFramework.Web.StartupInit
{
    public static class ExceptionBinderInitExtensions
    {
        public static void UseExceptionBinder_Local(this IApplicationBuilder app, bool isDebug)
        {
            app.UseMvcExceptionHandler((s) =>
            {
                s.IsDebug = isDebug;
                s.DefaultHttpCode = 500;
                s.CanBindByHttpCode = true;
                s.Host = Env.ExternalUrl;
                s.JsonSerializerSettings.Formatting = isDebug ? Formatting.Indented : Formatting.None;

                s.Mapping((builder) =>
                {
                    //Регистрируем исключение по http коду
                    builder.Register(httpCode: 500,
                        errorKey: "InternalServerError"
                    );
                    builder.Register(httpCode: 403,
                        errorKey: "Forbidden"
                    );
                    builder.Register(httpCode: 400,
                        errorKey: "BadRequest"
                    );
                    builder.Register<UnauthorizedAccessException>(httpCode: 401,
                        errorKey: "Unauthorized"
                    );
                    builder.RegisterAllAssignable<Exception>(httpCode: 500,
                        errorKeyPrefix: ""
                    );
                });
            });
        }
    }
}