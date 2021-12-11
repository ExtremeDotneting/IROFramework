using System.Collections.Generic;
using System.Threading.Tasks;
using IROFramework.Core.Consts;
using IROFramework.Core.Tools.AbstractDatabase;
using IROFramework.Web.Dto.CrudDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IROFramework.Web.Controllers.Crud
{
    public abstract class BaseCrudController<TModel, TId> : ControllerBase
        where TModel : class, IBaseModel<TId>
    {
        readonly IDatabaseSet<TModel, TId> _dbSet;

        protected BaseCrudController(IAbstractDatabase db)
        {
            _dbSet = db.GetDbSet<TModel, TId>();
        }

        [Authorize(Roles =UserRoles.Admin)]
        [HttpGet("get")]
        public async Task<TModel> GetById([FromQuery] TId id)
        {
            return await _dbSet.GetByIdAsync(id);
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpGet("getByPropertyValue")]
        public async Task<TModel> GetByPropertyAsync(string propName, string value)
        { ;
            return await _dbSet.GetByPropertyAsync(propName, value);
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpGet("getAll")]
        public async Task<IEnumerable<TModel>> GetAllAsync()
        {
            return await _dbSet.GetAllAsync();
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost("insert")]
        public async Task InsertAsync(TModel model)
        {
            await _dbSet.InsertAsync(model);
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost("update")]
        public async Task UpdateAsync(TModel model)
        {
            await _dbSet.UpdateAsync(model);
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost("delete")]
        public async Task DeleteAsync(IdRequest<TId> dto)
        {
            await _dbSet.DeleteAsync(dto.Id);
        }

    }
}