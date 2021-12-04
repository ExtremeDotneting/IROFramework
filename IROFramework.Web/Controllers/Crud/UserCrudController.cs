using System;
using IROFramework.Core.AppEnvironment;
using IROFramework.Core.Models;
using IROFramework.Core.Tools.AbstractDatabase;
using Microsoft.AspNetCore.Mvc;

namespace IROFramework.Web.Controllers.Crud
{
    [ApiController]
    [Route(CommonConsts.ApiPath + "/crud/user")]
    public class UserCrudController : BaseCrudController<UserModel, Guid>
    {
        public UserCrudController(IAbstractDatabase db) : base(db)
        {
        }
    }
}