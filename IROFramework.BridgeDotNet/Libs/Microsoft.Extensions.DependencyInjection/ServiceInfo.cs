using System;

namespace Microsoft.Extensions.DependencyInjection
{
    class ServiceInfo
    {
        public bool IsSingleton { get; set; }

        public object SavedInstance { get; set; }

        public Func<ICustomServiceProvider, object> Factory { get; set; }
    }
}
