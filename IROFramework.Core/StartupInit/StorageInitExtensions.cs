using System;
using System.IO;
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
            else if (type == StorageType.None)
            {
                return;
            }
            else
            {
                throw new Exception("Can't recognize StorageType settings.");
            }
        }

        internal static void AddTelegramBot(IServiceCollection services)
        {
            var storageOnTelegramSettings = Env.GetValue<StorageOnTelegramSettings>();
            //Telegram bot part.
            var bot = new TelegramBotClient(
                storageOnTelegramSettings.BotToken,
                new QueuedHttpClient(TimeSpan.FromMilliseconds(50))
            );
            services.AddSingleton<ITelegramBotClient>(bot);
        }

        static void AddTelegramStorage(this IServiceCollection services)
        {
            AddTelegramBot(services);

            var appSettings = Env.GetValue<StorageOnTelegramSettings>();
            //Telegram storage part.
            var opt = new TelegramStorageOptions()
            {
                SaveResourcesChatId = appSettings.SaveResourcesChatId,
                SaveOnSet = appSettings.SaveOnSet,
                LoadOnGet = appSettings.LoadOnGet,
                //AutoSavesDelay = TimeSpan.FromSeconds(appSettings.AutoSavesDelaySeconds),
                DeletePreviousMessages = appSettings.DeletePreviousMessages
            };
            services.AddSingleton(opt);
            services.AddSingleton<IKeyValueStorage, TelegramStorage>();
        }

        static void AddRamStorage(this IServiceCollection services)
        {
            services.AddSingleton<IKeyValueStorage>(new RamStorage());
        }

    }
}
