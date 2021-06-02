using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public interface ICustomServiceProvider
    {
        object GetService(Type serviceType);
    }
}