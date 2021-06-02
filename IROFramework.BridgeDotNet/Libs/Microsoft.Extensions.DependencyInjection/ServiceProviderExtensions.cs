using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceProviderExtensions
    {
        public static object GetRequiredService(this ICustomServiceProvider @this, Type serviceType)
        {
            var serv = @this.GetService(serviceType);
            if (serv == null)
            {
                throw new Exception($"Service '{serviceType.FullName}' not found in ioc.");
            }
            return serv;
        }

        public static TService GetRequiredService<TService>(this ICustomServiceProvider @this)
        {
            return @this
                .GetRequiredService(typeof(TService))
                .Cast<TService>();
        }
    }
}