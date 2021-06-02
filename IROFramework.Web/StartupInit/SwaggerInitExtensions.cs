using System.Reflection;
using IRO.Mvc.CoolSwagger;
using IROFramework.Core.AppEnvironment;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace IROFramework.Web.StartupInit
{
    public static class SwaggerInitExtensions
    {
        static string _swaggerTitle = Assembly.GetExecutingAssembly().GetName().Name;

        public static void UseSwaggerUI_Local(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                
                c.ShowExtensions();
                c.EnableValidator();
                c.SwaggerEndpoint($"/swagger/v1/swagger.json", _swaggerTitle);
                c.DisplayOperationId();
                c.DisplayRequestDuration();
            });


        }
        public static void AddSwaggerGen_Local(this IServiceCollection services)
        {
            services.AddSwaggerGen(opt =>
            {
                var host = System.Net.Dns.GetHostName();
                string assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                var swaggerDescr = $"* Environment: {Env.EnvironmentName}\n* Assembly version: {assemblyVersion}\n* Host: {host}.";

                opt.SwaggerDoc(
                    "v1", 
                    new OpenApiInfo
                    {
                        Title = _swaggerTitle,
                        Version = assemblyVersion,
                        Description = swaggerDescr
                    });
                opt.EnableAnnotations();
                opt.UseCoolSummaryGen();
                opt.UseDefaultIdentityAuthScheme();
                opt.AddSwaggerTagNameOperationFilter();
                opt.AddDefaultResponses(new ResponseDescription()
                {
                    StatusCode = 500,
                    Description = "Server visible error."
                });

            });
        }
    }
}
