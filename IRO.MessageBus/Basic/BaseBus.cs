using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ConcurrentCollections;
using IRO.MessageBus.Abstract;
using IRO.MessageBus.Metadata;

namespace IRO.MessageBus.Basic
{
    class DirectBusHandlerInfo
    {
        public string FullRequestName { get; set; }

        public string RequestName { get; set; }

        public Type HandlerType { get; set; }

        public Type RequestType { get; set; }

        public MethodInfo MethodInfo { get; set; }

    }

    public class DirectBus : IBus
    {
        private readonly ConcurrentHashSet<Assembly> _usedAssemblies
            = new ConcurrentHashSet<Assembly>();
        private readonly IDictionary<string, DirectBusHandlerInfo> _busHandlerInfosByFullAndShortReqName
            = new ConcurrentDictionary<string, DirectBusHandlerInfo>();
        private readonly ConcurrentHashSet<string> _handlersRequestsFullNames
            = new ConcurrentHashSet<string>();
        private readonly IDictionary<Type, string> _fullRequestNameByRequestType
            = new ConcurrentDictionary<Type, string>();
        private readonly IServiceProvider _serviceProvider;

        public DirectBus(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        protected virtual void RegisterAssembly(Assembly[] searchDirectHandlersInAssemblies = null)
        {
            searchDirectHandlersInAssemblies ??= AppDomain.CurrentDomain.GetAssemblies();
            var localHandlers = GetAvailableRequestHandlers(searchDirectHandlersInAssemblies);
            foreach (var handlerType in localHandlers)
            {
                var busHandlerInfo = GetBusHandlerInfo(handlerType);
                _busHandlerInfosByFullReqName[busHandlerInfo.FullRequestName] = busHandlerInfo;
                _handlersRequestsFullNames.Add(busHandlerInfo.FullRequestName);
            }
        }

        public Task Call<TBusRequest>(TBusRequest req, CancellationToken cancellationToken = default)
        {

        }

        public Task Call(string requestName, IBusRequest request, CancellationToken cancellationToken = default)
        {

        }

        public void IsHandlerAvalilable(string requestFullName)
        {
            if (_busHandlerInfosByFullReqName.)
        }

        string GetFullRequestName(Type requestType)
        {
            if (_fullRequestNameByRequestType.TryGetValue(requestType, out var reqName))
            {
                return reqName;
            }
            else
            {
                var value = requestType.Namespace + "." + GetRequestName(requestType);
                _fullRequestNameByRequestType[requestType] = value;
                return value;
            }
        }

        string GetRequestName(Type requestType)
        {
            var attr = requestType.GetCustomAttribute<BusRequestAttribute>();
            if (attr == null)
            {
                return requestType.Name;
            }
            else
            {
                return attr.RequestName;
            }
        }

        DirectBusHandlerInfo GetBusHandlerInfo(Type handlerType)
        {
            var info = new DirectBusHandlerInfo();
            var requestType = handlerType.GetGenericArguments()[0];
            info.RequestName = GetRequestName(requestType);
            info.FullRequestName = GetFullRequestName(requestType);
            info.MethodInfo = handlerType.GetMethod("Handle");
            return info;
        }

        IEnumerable<Type> GetAvailableRequestHandlers(Assembly[] searchDirectHandlersInAssemblies)
        {
            var baseType = typeof(IBusHandler);
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var requestsHandlers = assemblies
                .SelectMany(s => s.GetTypes())
                .Where(t => t.IsClass && !t.IsAbstract && baseType.IsAssignableFrom(t));
            return requestsHandlers;
        }
    }
}
