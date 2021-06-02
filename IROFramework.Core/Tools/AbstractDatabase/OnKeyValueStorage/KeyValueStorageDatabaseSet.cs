using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IRO.Storage;
using NeoSmart.AsyncLock;

namespace IROFramework.Core.Tools.AbstractDatabase.OnKeyValueStorage
{
    /// <summary>
    /// Store data in key-value storage. Just for tests.
    /// </summary>
    public class KeyValueStorageDatabaseSet<TModel, TId> : IDatabaseSet<TModel, TId>
        where TModel : IBaseModel<TId>
    {
        readonly ConcurrentDictionary<TId, TModel> _dict;
        readonly IKeyValueStorage _storage;
        readonly string _colName;
        readonly AsyncLock _lock = new AsyncLock();

        public KeyValueStorageDatabaseSet(IKeyValueStorage storage, string colName)
        {
            _storage = storage;
            _colName = colName;
            _dict= _storage.GetOrDefault<ConcurrentDictionary<TId, TModel>>("AbstractDatabaseCollention_" + colName)
                .Result;
            _dict??= new ConcurrentDictionary<TId, TModel>();
        }

        public async Task<TModel> GetByIdAsync(TId id)
        {
            using (await _lock.LockAsync())
            {
                return _dict[id];
            }
        }

        public async Task<TModel> GetByPropertyAsync(string propName, object value)
        {
            using (await _lock.LockAsync())
            {
                var model = _dict
                    .ToArray()
                    .First((r) =>
                    {
                        var propValue = GetPropertyValue(r.Value, propName);
                        return propValue.Equals(value);
                    });
                return model.Value;
            }
        }

        public async Task<IEnumerable<TModel>> GetAllAsync()
        {
            using (await _lock.LockAsync())
            {
                return _dict
                    .ToArray()
                    .Select(r => r.Value);
            }
        }

        public async Task InsertAsync(TModel model)
        {
            using (await _lock.LockAsync())
            {
                if (!_dict.TryAdd(model.Id, model))
                {
                    throw new Exception("Can't add model to dict.");
                }
                await SaveStorage();
            }
        }

        public async Task UpdateAsync(TModel model)
        {
            using (await _lock.LockAsync())
            {
                _dict[model.Id] = model;
                await SaveStorage();
            }
        }

        public async Task DeleteAsync(TId id)
        {
            using (await _lock.LockAsync())
            {
                _dict.TryRemove(id, out var val);
                await SaveStorage();
            }
        }

        public async Task EnsureIndex(string propertyName)
        {
            throw new NotSupportedException();
        }

        async Task SaveStorage()
        {
            await _storage.Set("AbstractDatabaseCollention_" + _colName, _dict);
        }

        object GetPropertyValue(TModel model, string propName)
        {
            var prop = model.GetType().GetProperty(propName);
            if (prop == null)
            {
                throw new Exception($"Property with name {propName} not found.");
            }
            return prop.GetValue(model);
        }
    }
}