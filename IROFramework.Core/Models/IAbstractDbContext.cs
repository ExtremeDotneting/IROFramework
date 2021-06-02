using System;
using IROFramework.Core.Tools.AbstractDatabase;

namespace IROFramework.Core.Models
{
    public interface IAbstractDbContext
    {
        IDatabaseSet<UserModel, Guid> Users { get; }
    }
}