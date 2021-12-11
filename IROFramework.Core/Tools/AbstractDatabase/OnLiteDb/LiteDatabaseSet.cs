using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiteDB;

namespace IROFramework.Core.Tools.AbstractDatabase.OnLiteDb
{
    public class LiteDatabaseSet<TModel, TId> : IDatabaseSet<TModel, TId>
        where TModel : class, IBaseModel<TId>
    {
        readonly Func<LiteDatabase> _factory;
        readonly string _colName;

        public LiteDatabaseSet(Func<LiteDatabase> factory, string colName)
        {
            _factory = factory;
            _colName = colName;
        }

        public async Task<TModel> TryGetByIdAsync(TId id)
        {
            using (var db = _factory())
            {
                var col = db.GetCollection<TModel>(_colName);
                var res = col.FindById(new BsonValue(id));
                return res;
            }
        }

        public async Task<TModel> TryGetByPropertyAsync(string propName, object value)
        {
            using (var db = _factory())
            {
                var col = db.GetCollection<TModel>(_colName);
                var query=Query.EQ(propName, new BsonValue(value));
                var res = col.FindOne(query);
                return res;
            }
        }

        public async Task<IEnumerable<TModel>> GetAllAsync()
        {
            using (var db = _factory())
            {
                var col = db.GetCollection<TModel>(_colName);
                var res = col.FindAll();
                return res;
            }
        }

        public async Task InsertAsync(TModel model)
        {
            using (var db = _factory())
            {
                var col = db.GetCollection<TModel>(_colName);
                col.Insert(model);
            }
        }

        public async Task UpdateAsync(TModel model)
        {
            using (var db = _factory())
            {
                var col = db.GetCollection<TModel>(_colName);
                col.Update(model);
            }
        }

        public async Task DeleteAsync(TId id)
        {
            using (var db = _factory())
            {
                var col = db.GetCollection<TModel>(_colName);
                var query = Query.EQ("Id", new BsonValue(id));
                col.Delete(new BsonValue(id));
            }
        }

        public async Task EnsureIndex(string propertyName)
        {
            using (var db = _factory())
            {
                var col = db.GetCollection<TModel>(_colName);
                col.EnsureIndex(propertyName);
            }
        }

        public IDisposable UsingCollection(out ILiteCollection<TModel> col)
        {
            var db = _factory();
            col = db.GetCollection<TModel>(_colName);
            return db;
        }
    }
}