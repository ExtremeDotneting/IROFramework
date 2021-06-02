using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions 
    {
        public static void AddSingleton<TService>(this IServiceCollection serviceCollection, TService inst) 
            where TService : class
        {
            serviceCollection.AddSingleton(typeof(TService), inst);
        }

        public static void AddSingleton<TService>(this IServiceCollection serviceCollection, Func<ICustomServiceProvider, TService> factory)
        {
            serviceCollection.AddSingleton(
                typeof(TService),
                (sp) => factory(sp)
            );
        }

        public static void AddSingleton<TBase, TImplemention>(this IServiceCollection serviceCollection)
            where TImplemention : TBase
        {
            serviceCollection.AddSingleton(typeof(TBase), typeof(TImplemention));
        }

        public static void AddSingleton<TService>(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton(typeof(TService), typeof(TService));
        }

        public static void AddSingleton(this IServiceCollection serviceCollection, Type serviceType)
        {
            serviceCollection.AddSingleton(serviceType, serviceType);
        }
        
        public static void AddTransient<TService>(this IServiceCollection serviceCollection, Func<ICustomServiceProvider, TService> factory)
        {
            serviceCollection.AddTransient(
                typeof(TService),
                (sp) => factory(sp)
                );
        }

        public static void AddTransient<TBase, TImplemention>(this IServiceCollection serviceCollection)
            where TImplemention : TBase
        {
            serviceCollection.AddTransient(typeof(TBase), typeof(TImplemention));
        }

        public static void AddTransient<TService>(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient(typeof(TService), typeof(TService));
        }

        public static void AddTransient(this IServiceCollection serviceCollection, Type serviceType)
        {
            serviceCollection.AddTransient(serviceType, serviceType);
        }

    }



}