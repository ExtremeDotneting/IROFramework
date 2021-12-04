using System;
using System.IO;
using IROFramework.Core.AppEnvironment;
using IROFramework.Core.Consts;
using IROFramework.Core.Tools.AbstractDatabase;
using IROFramework.Core.Tools.AbstractDatabase.OnKeyValueStorage;
using IROFramework.Core.Tools.AbstractDatabase.OnLiteDb;
using LiteDB;
using Microsoft.Extensions.DependencyInjection;

namespace IROFramework.Core.StartupInit
{
    public static class AbstractDatabaseInit
    {
        public static void AddMyAbstractDatabase(this IServiceCollection services)
        {
            var dbType=Env.GlobalSettings.DatabaseType;
            if (dbType == DatabaseType.KeyValue)
            {
                services.AddMyKeyValueDatabase();
            }
            else if (dbType == DatabaseType.LiteDB)
            {
                services.AddMyLiteDatabase();
            }
            else if (dbType == DatabaseType.None)
            {
                return;
            }
            else
            {
                throw new Exception("Can't recognize DatabaseType settings.");
            }
        }

        static void AddMyLiteDatabase(this IServiceCollection services, string databasePath = null)
        {
            var dbFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "storage.litedb");
            services.AddSingleton<IAbstractDatabase>(
                new AbstractLiteDatabase(() => new LiteDatabase(dbFilePath))
                );
        }

        static void AddMyKeyValueDatabase(this IServiceCollection services)
        {
            services.AddSingleton<IAbstractDatabase, AbstractKeyValueStorageDatabase>();
        }
    }

}
