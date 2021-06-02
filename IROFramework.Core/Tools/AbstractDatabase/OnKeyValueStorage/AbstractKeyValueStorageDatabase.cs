using System;
using IRO.Storage;

namespace IROFramework.Core.Tools.AbstractDatabase.OnKeyValueStorage
{
    public class AbstractKeyValueStorageDatabase : IAbstractDatabase
    {
        readonly IKeyValueStorage _storage;

        public AbstractKeyValueStorageDatabase(IKeyValueStorage storage)
        {
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
        }

        public IDatabaseSet<TModel, TId> GetDbSet<TModel, TId>(string name) where TModel : IBaseModel<TId>
        {
            return new KeyValueStorageDatabaseSet<TModel, TId>(_storage, name);
        }

        public IDatabaseSet<TModel, TId> GetDbSet<TModel, TId>() where TModel : IBaseModel<TId>
        {
            var name = typeof(TModel).Name;
            return GetDbSet<TModel, TId>(name);
        }
    }
}