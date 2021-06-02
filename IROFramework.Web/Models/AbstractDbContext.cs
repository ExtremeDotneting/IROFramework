using System;
using System.Collections.Generic;
using System.Text;
using IROFramework.Core.Models;
using IROFramework.Core.Tools.AbstractDatabase;

namespace IROFramework.Web.Models
{
    public class AbstractDbContext : IAbstractDbContext
    {
        public IDatabaseSet<UserModel, Guid> Users { get; }

        public AbstractDbContext(IAbstractDatabase abstractDb)
        {
            Users = abstractDb.GetDbSet<UserModel, Guid>();

        }


    }
}
