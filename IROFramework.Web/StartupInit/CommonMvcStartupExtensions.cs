using System;
using IRO.Storage;
using IROFramework.Core.AppEnvironment;
using IROFramework.Core.AppEnvironment.SettingsDto;
using IROFramework.Web.Dto.FilesStorageDto;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.AspNetPipeline.Extensions;
using Telegram.Bot.CloudStorage;
using Telegram.Bot.CloudStorage.Data;

namespace IROFramework.Web.StartupInit
{
    public static class CommonMvcStartupExtensions
    {
        public static void IncreaseMaxFileUploadSize(this IApplicationBuilder app)
        {
            //Increase file upload size.
            app.UseWhen(context => context.Request.Path.StartsWithSegments($"/{CommonConsts.ApiPath}/files/upload"),
                appBuilder =>
                {
                    appBuilder.Use(async (ctx, next) =>
                    {
                        ctx.Features.Get<IHttpMaxRequestBodySizeFeature>().MaxRequestBodySize = CommonConsts.MaxFileSize;
                        await next();
                    });
                });
        }

        public static void ConfigureFormMaxUploadSize(this IServiceCollection services)
        {
            //Increase file upload size.
            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = CommonConsts.MaxFileSize;
                x.MultipartBodyLengthLimit = CommonConsts.MaxFileSize; // In case of multipart
            });
        }

        public static void AddTelegramFilesCloud(this IServiceCollection services)
        {
            var opt = Env.GetValue<StorageOnTelegramSettings>();
            //Telegram bot part.
            var bot = new TelegramBotClient(
                opt.BotToken,
                new QueuedHttpClient(TimeSpan.FromMilliseconds(50))
            );
            services.AddSingleton<ITelegramBotClient>(bot);

            //Telegram files limit is 50 mb.
            //Note that with CacheAndNotWait you can operate with any files, but too big files will be not saved in telegram,
            //so they will be unavailable after app restart.
            services.AddSingleton(new TelegramFilesCloudOptions()
            {
                SaveResourcesChatId = Env.GetValue< StorageOnTelegramSettings>().SaveResourcesChatId,
                CacheAndNotWait = true,
                DeleteOlderFiles = false
            });
            services.AddSingleton<TelegramFilesCloud<FileMetadata>>();
        }

        /// <summary>
        /// Redirect site root '/' to '/index.html'.
        /// </summary>
        public static void UseIndexHtmlRedirect(this IApplicationBuilder app)
        {
            app.Use(async (ctx, next) =>
            {
                var url = ctx.Request.Path.Value;
                if (string.IsNullOrEmpty(url) || url.Trim() == "/")
                {
                    ctx.Response.Redirect(Env.ExternalUrl + "/index.html");
                }
                else
                {
                    await next();
                }
            });
        }

    }
}
