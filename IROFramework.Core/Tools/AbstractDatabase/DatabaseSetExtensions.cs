using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace IROFramework.Core.Tools.AbstractDatabase
{
    public static class DatabaseSetExtensions
    {
        public static async Task<TModel> GetByIdAsync<TModel, TId>(
            this IDatabaseSet<TModel, TId> dbSet,
            TId id
        )
            where TModel : class, IBaseModel<TId>
        {
            var model=await dbSet.TryGetByIdAsync(id);
            if (model == null)
            {
                throw new NullReferenceException($"Record with id '{id}' not found.");
            }
            return model;
        }

        public static async Task<TModel> GetByPropertyAsync<TModel, TId>(
            this IDatabaseSet<TModel, TId> dbSet,
            string propName, 
            object value
        )
            where TModel : class, IBaseModel<TId>
        {
            var model = await dbSet.TryGetByPropertyAsync(propName, value);
            if (model == null)
            {
                throw new NullReferenceException($"Record with '{propName}' == '{value}' not found.");
            }
            return model;
        }

        public static async Task<TModel> TryGetByPropertyAsync<TModel, TId>(
            this IDatabaseSet<TModel, TId> dbSet,
            Expression<Func<TModel, object>> nameExpr,
            object value
        )
            where TModel : class, IBaseModel<TId>
        {
            //Resolving name.
            string name = null;
            if (nameExpr.Body is MemberExpression memberExpr)
            {
                name = memberExpr.Member.Name;
            }
            else if (nameExpr.Body is UnaryExpression unaryExpr)
            {
                var memberExpr2 = (MemberExpression)unaryExpr.Operand;
                name = memberExpr2.Member.Name;
            }
            else
            {
                throw new Exception("Can't resolve member name from expression.");
            }
            return await dbSet.TryGetByPropertyAsync(name, value);
        }

        public static async Task<TModel> GetByPropertyAsync<TModel, TId>(
            this IDatabaseSet<TModel, TId> dbSet, 
            Expression<Func<TModel, object>> nameExpr,
            object value
            )
            where TModel : class, IBaseModel<TId>
        {
            var model = await TryGetByPropertyAsync<TModel, TId>(dbSet, nameExpr, value);
            if (model == null)
            {
                throw new NullReferenceException($"Record with property value '{value}' not found.");
            }
            return model;
        }
    }
}