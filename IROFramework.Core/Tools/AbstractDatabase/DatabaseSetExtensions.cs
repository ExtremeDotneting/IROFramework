using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace IROFramework.Core.Tools.AbstractDatabase
{
    public static class DatabaseSetExtensions
    {
        public static async Task<TModel> TryGetByIdAsync<TModel, TId>(
            this IDatabaseSet<TModel, TId> dbSet,
            TId id
        )
            where TModel : IBaseModel<TId>
        {
            try
            {
                return await dbSet.GetByIdAsync(id);
            }
            catch
            {
                return default(TModel);
            }
        }

        public static async Task<TModel> TryGetByPropertyAsync<TModel, TId>(
            this IDatabaseSet<TModel, TId> dbSet,
            string propName, 
            object value
        )
            where TModel : IBaseModel<TId>
        {
            try
            {
                return await dbSet.GetByPropertyAsync(propName, value);
            }
            catch
            {
                return default(TModel);
            }
        }

        public static async Task<TModel> TryGetByPropertyAsync<TModel, TId>(
            this IDatabaseSet<TModel, TId> dbSet,
            Expression<Func<TModel, object>> nameExpr,
            object value
        )
            where TModel : IBaseModel<TId>
        {
            try
            {
                return await dbSet.GetByPropertyAsync(nameExpr, value);
            }
            catch
            {
                return default(TModel);
            }
        }

        public static async Task<TModel> GetByPropertyAsync<TModel, TId>(
            this IDatabaseSet<TModel, TId> dbSet, 
            Expression<Func<TModel, object>> nameExpr,
            object value
            )
            where TModel : IBaseModel<TId>
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

            return await dbSet.GetByPropertyAsync(name, value);
        }
    }
}