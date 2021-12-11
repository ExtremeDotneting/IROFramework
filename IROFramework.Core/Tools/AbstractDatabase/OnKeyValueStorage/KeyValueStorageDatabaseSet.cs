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
        where TModel : class, IBaseModel<TId>
    {
        ConcurrentDictionary<TId, TModel> _dict;
        readonly IKeyValueStorage _storage;
        readonly string _colName;
        readonly AsyncLock _lock = new AsyncLock();

        public KeyValueStorageDatabaseSet(IKeyValueStorage storage, string colName)
        {
            _storage = storage;
            _colName = colName;
            var t = Task.Run(async () =>
            {
                using (await _lock.LockAsync())
                {
                    _dict = await _storage
                        .GetOrDefault<ConcurrentDictionary<TId, TModel>>("AbstractDatabaseCollection_" + colName)
                        .ConfigureAwait(false);
                    if (_dict == null)
                    {
                        _dict = new ConcurrentDictionary<TId, TModel>();
                    }
                }
            });
        }

        public async Task<TModel> TryGetByIdAsync(TId id)
        {
            using (await _lock.LockAsync())
            {
                if (_dict.TryGetValue(id, out var value))
                {
                    return value;
                }
                else
                {
                    return null;
                }
            }
        }

        public async Task<TModel> TryGetByPropertyAsync(string propName, object value)
        {
            using (await _lock.LockAsync())
            {
                var model = _dict
                    .ToArray()
                    .Select(p=>p.Value)
                    .FirstOrDefault((r) =>
                    {
                        var propValue = GetPropertyValue(r, propName);
                        return propValue.Equals(value);
                    });
                return model;
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
            await _storage.Set("AbstractDatabaseCollection_" + _colName, _dict);
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