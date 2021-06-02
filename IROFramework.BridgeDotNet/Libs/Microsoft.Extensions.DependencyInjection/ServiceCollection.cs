using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection
{
    public class ServiceCollection : IServiceCollection, ICustomServiceProvider
    {
        #region From base class
        bool IsBuilded;

        bool RegisteredNew;

        public bool ThrowExceptionIfRegisteredAfterResolve { get; set; }

        void BeforeResolve()
        {
            if (!RegisteredNew)
            {
                return;
            }

            if (IsBuilded)
            {
                if (ThrowExceptionIfRegisteredAfterResolve)
                {
                    throw new Exception("Register new types after resolve is called is bad practice. " +
                                        "You can try ignore current exception if you set BaseIocSystem.ThrowExceptionIfRegisteredAfterResolve " +
                                        "to true or if you switch to release mode.");
                }

            }
            else
            {
                Build();
                IsBuilded = true;
            }

            RegisteredNew = false;
        }

        void OnRegister()
        {
            RegisteredNew = true;
        }

        public void Update()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Вернет ссылку на самого себя.
        /// </summary>
        public ICustomServiceProvider Build()
        {
            IsBuilded = true;
            return this;
        }

        #endregion

        /// <summary>
        /// Allow user to register type that was registered.
        /// If false - ioc will throw exception.
        /// </summary>
        public bool AllowRegisterRegisteredTypes { get; set; }

        readonly Dictionary<Type, ServiceInfo> _resolvers = new Dictionary<Type, ServiceInfo>();

        public void AddTransient(Type baseType, Func<ICustomServiceProvider, object> func)
        {
            CheckAlreadyAdded(baseType);
            var info = new ServiceInfo()
            {
                Factory = func,
                IsSingleton = false
            };
            _resolvers[baseType] = info;
            OnRegister();
        }

        public void AddTransient(Type baseType, Type inheritType)
        {
            CheckAlreadyAdded(baseType);
            CheckTypeMatching(baseType, inheritType);
            var info = new ServiceInfo()
            {
                Factory = (sp) => CreateService(sp, inheritType),
                IsSingleton = false
            };
            _resolvers[baseType] = info;
            if (!_resolvers.ContainsKey(inheritType))
                _resolvers.Add(inheritType, info);

            OnRegister();
        }

        public void AddSingleton(Type baseType, Type inheritType)
        {
            CheckAlreadyAdded(baseType);
            CheckTypeMatching(baseType, inheritType);
            var info = new ServiceInfo()
            {
                Factory = (sp) => CreateService(sp, inheritType),
                IsSingleton = true
            };
            _resolvers[baseType] = info;
            if (!_resolvers.ContainsKey(inheritType))
                _resolvers.Add(inheritType, info);
            OnRegister();
        }

        public void AddSingleton(Type baseType, Func<ICustomServiceProvider, object> func)
        {
            CheckAlreadyAdded(baseType);
            var info = new ServiceInfo()
            {
                Factory = func,
                IsSingleton = true
            };
            _resolvers[baseType] = info;
            OnRegister();
        }

        public void AddSingleton(Type baseType, object inst)
        {
            CheckAlreadyAdded(baseType);

            if (inst == null)
            {
                throw new ArgumentNullException("instance", $"Instance of {baseType.Name} can`t be null.");
            }
            CheckTypeMatching(baseType, inst.GetType());

            var info = new ServiceInfo()
            {
                SavedInstance = inst,
                IsSingleton = true
            };
            _resolvers[baseType] = info;

            OnRegister();
        }

        public object GetService(Type type)
        {
            BeforeResolve();
            if (!_resolvers.ContainsKey(type))
                return null;
            
            var serviceInfo = _resolvers[type];
            if (serviceInfo.IsSingleton)
            {
                if (serviceInfo.SavedInstance == null)
                {
                    serviceInfo.SavedInstance = serviceInfo.Factory(this);
                }
                return serviceInfo.SavedInstance;
            }
            else
            {
                return serviceInfo.Factory(this);
            }
        }

        #region PRIVATE

        static object CreateService(ICustomServiceProvider sp, Type serviceType)
        {
            // get ctor
            var constructors = serviceType.GetConstructors();
            var ctor = constructors[0];
            if (ctor == null)
                throw new Exception($"No constructor found for type {serviceType.FullName}!");

            // get ctor params
            var ctorParams = ctor.GetParameters();
            if (ctorParams.Length==0)
                return Activator.CreateInstance(serviceType);
            else
            {
                // recursive resolve
                var parameters = new List<object>(ctorParams.Length);

                foreach (var parameterInfo in ctorParams)
                    parameters.Add(sp.GetService(parameterInfo.ParameterType));
                return Activator.CreateInstance(serviceType, parameters.ToArray());
            }
        }

        void CheckAlreadyAdded(Type type)
        {
            if (_resolvers.ContainsKey(type) && AllowRegisterRegisteredTypes)
                throw new Exception($"'{type.Name}' is already registered!");
        }

        void CheckTypeMatching(Type baseType, Type inheritType)
        {
            if (!baseType.IsAssignableFrom(inheritType))
                throw new Exception($"'{baseType.Name}' is not assignable from '{inheritType.Name}'.");
        }
        #endregion
    }
}