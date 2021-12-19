using System.Collections.Generic;
using System.Threading.Tasks;

namespace IROFramework.Core.Tools.AbstractDatabase
{
    public interface IDatabaseSet<TModel, in TId>
        where TModel : class, IBaseModel<TId>
    {
        Task<TModel> TryGetByIdAsync(TId id);

        Task<TModel> TryGetOneByPropertyAsync(string propName, object value);

        Task<IEnumerable<TModel>> GetByPropertyAsync(string propName, object value);

        Task<IEnumerable<TModel>> GetAllAsync();

        Task InsertAsync(TModel model);

        Task UpdateAsync(TModel model);

        Task DeleteAsync(TId id);

        Task EnsureIndex(string propertyName);
    }
}