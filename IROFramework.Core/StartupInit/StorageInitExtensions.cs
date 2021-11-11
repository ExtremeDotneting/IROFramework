using System;
using IRO.Storage;
using IRO.Storage.DefaultStorages;
using IROFramework.Core.AppEnvironment;
using IROFramework.Core.AppEnvironment.SettingsDto;
using IROFramework.Core.Consts;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.AspNetPipeline.Extensions;
using Telegram.Bot.CloudStorage;
using Telegram.Bot.CloudStorage.Data;

namespace IROFramework.Core.StartupInit
{
    public static class StorageInitExtensions
    {
        public static void AddMyStorage(this IServiceCollection services)
        {
            var type = Env.GlobalSettings.StorageType;
            if (type == StorageType.Ram)
            {
                services.AddRamStorage();
            }
            else if (type == StorageType.Telegram)
            {
                services.AddTelegramStorage();
            }
            else
            {
                throw new Exception("Can't recognize storage type settings.");
            }
        }

        public static void AddTelegramStorage(this IServiceCollection services)
        {
            //Telegram storage part.
            var opt = new TelegramStorageOptions()
            {
                SaveResourcesChatId = Env.GetValue<StorageOnTelegramSettings>().SaveResourcesChatId
            };
            services.AddSingleton(opt); 
            services.AddSingleton<IKeyValueStorage, TelegramStorage>();
        }

        public static void AddRamStorage(this IServiceCollection services)
        {
            services.AddSingleton<IKeyValueStorage>(new RamStorage());
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
                SaveResourcesChatId = Env.GetValue<StorageOnTelegramSettings>().SaveResourcesChatId,
                CacheAndNotWait = true,
                DeleteOlderFiles = false
            });
            services.AddSingleton<TelegramFilesCloud>();
        }
    }
}
