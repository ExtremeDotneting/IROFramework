using System;
using System.Collections.Generic;
using System.Text;
using IROFramework.Core.AppEnvironment;
using IROFramework.Core.AppEnvironment.SettingsDto;
using IROFramework.Core.Consts;
using IROFramework.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.AspNetPipeline.Extensions;
using Telegram.Bot.CloudStorage;
using Telegram.Bot.CloudStorage.Data;

namespace IROFramework.Core.StartupInit
{
    public static class FilesCloudInit
    {
        public static void AddTelegramFilesCloud(this IServiceCollection services)
        {
            if (Env.GlobalSettings.StorageType!= StorageType.Telegram)
            {
               StorageInitExtensions.AddTelegramBot(services);
            }

            var appSettings = Env.GetValue<StorageOnTelegramSettings>();

            //Telegram files limit is 50 mb.
            //Note that with CacheAndNotWait you can operate with any files, but too big files will be not saved in telegram,
            //so they will be unavailable after app restart.
            services.AddSingleton(new TelegramFilesCloudOptions()
            {
                SaveResourcesChatId = appSettings.SaveResourcesChatId,
                CacheAndNotWait = appSettings.CacheFilesAndNotWait,
                DeleteOlderFiles = appSettings.DeleteOlderFiles,
                UseCache = true
            });
            services.AddSingleton<TelegramFilesCloud<FileMetadata>>();
        }
    }
}
