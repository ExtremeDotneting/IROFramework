using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public interface IServiceCollection
    {
        void AddSingleton(Type type, Func<ICustomServiceProvider, object> func);

        void AddSingleton(Type type, object inst);

        void AddSingleton(Type baseType, Type inheritType);

        void AddTransient(Type type, Func<ICustomServiceProvider, object> func);

        void AddTransient(Type baseType, Type inheritType);

        ICustomServiceProvider Build();
    }



}