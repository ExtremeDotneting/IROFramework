using System;
using LiteDB;

namespace IROFramework.Core.Tools.AbstractDatabase.OnLiteDb
{
    public class AbstractLiteDatabase : IAbstractDatabase
    {
        readonly Func<LiteDatabase> _factory;

        public AbstractLiteDatabase(Func<LiteDatabase> factory)
        {
            _factory = factory;
        }

        public IDatabaseSet<TModel, TId> GetDbSet<TModel, TId>(string name) where TModel : IBaseModel<TId>
        {
            return new LiteDatabaseSet<TModel, TId>(_factory, name);
        }

        public IDatabaseSet<TModel, TId> GetDbSet<TModel, TId>() where TModel : IBaseModel<TId>
        {
            var name = typeof(TModel).Name;
            return GetDbSet<TModel, TId>(name);
        }
    }
}
