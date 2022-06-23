using System;
using System.Collections.Generic;
using System.Text;
using IROFramework.Core.AppEnvironment;
using IROFramework.Web.Tools.Email;
using Microsoft.Extensions.DependencyInjection;

namespace IROFramework.Web.StartupInit
{
    public static class EmailServicesExtensions
    {
        public static void AddEmailService(this IServiceCollection services)
        {
            var emServ = new EmailService(Env.GetValue<EmailOptions>());
            services.AddSingleton<IEmailService>(emServ);
        }
    }
}
