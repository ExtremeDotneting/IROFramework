using System;
using System.Collections.Concurrent;
using IRO.Storage;

namespace IROFramework.Core.Tools.AbstractDatabase.OnKeyValueStorage
{
    public class AbstractKeyValueStorageDatabase : IAbstractDatabase
    {
        readonly IKeyValueStorage _storage;
        readonly ConcurrentDictionary<string, object> _storagesObjects = new ConcurrentDictionary<string, object>();

        public AbstractKeyValueStorageDatabase(IKeyValueStorage storage)
        {
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
        }

        public IDatabaseSet<TModel, TId> GetDbSet<TModel, TId>(string name) where TModel : IBaseModel<TId>
        {
            if (_storagesObjects.TryGetValue(name, out var storageObj))
            {
                return (IDatabaseSet<TModel, TId>) storageObj;
            }
            else
            {
                var newStorage=new KeyValueStorageDatabaseSet<TModel, TId>(_storage, name);
                _storagesObjects[name] = newStorage;
                return newStorage;
            }
        }

        public IDatabaseSet<TModel, TId> GetDbSet<TModel, TId>() where TModel : IBaseModel<TId>
        {
            var name = typeof(TModel).Name;
            return GetDbSet<TModel, TId>(name);
        }
    }
}