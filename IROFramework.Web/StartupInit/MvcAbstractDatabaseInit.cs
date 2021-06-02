using System;
using System.IO;
using IROFramework.Core.Models;
using IROFramework.Core.Tools.AbstractDatabase;
using IROFramework.Core.Tools.AbstractDatabase.OnKeyValueStorage;
using IROFramework.Core.Tools.AbstractDatabase.OnLiteDb;
using IROFramework.Web.Models;
using LiteDB;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace IROFramework.Web.StartupInit
{
    public static class MvcAbstractDatabaseInit
    {
        public static void AddMyLiteDatabase(this IServiceCollection services, string databasePath = null)
        {
            var dbFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "storage.litedb");
            services.AddSingleton<IAbstractDatabase>(
                new AbstractLiteDatabase(() => new LiteDatabase(dbFilePath))
                );
        }

        public static void UseMyLiteDatabase(this IApplicationBuilder app)
        {
            var serv = app.ApplicationServices;
            var usersSet = serv.GetRequiredService<IDatabaseSet<UserModel, Guid>>();
            usersSet.EnsureIndex("Token");
            usersSet.EnsureIndex("Nickname");
        }

        public static void AddMyKeyValueDatabase(this IServiceCollection services)
        {
            services.AddSingleton<IAbstractDatabase, AbstractKeyValueStorageDatabase>();
        }

        public static void AddAbstractDbContext(this IServiceCollection services)
        {
            services.AddSingleton<AbstractDbContext>((sp) =>
            {
                var db = sp.GetRequiredService<IAbstractDatabase>();
                var dbContext = new AbstractDbContext(db);
                return dbContext;
            });

            services.AddSingleton<IDatabaseSet<UserModel, Guid>>((sp) =>
            {
                var dbCtx = sp.GetRequiredService<AbstractDbContext>();
                return dbCtx.Users;
            });
        }



    }

}
