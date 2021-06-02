namespace IROFramework.Core.Tools.AbstractDatabase
{
    public interface IAbstractDatabase
    {
        IDatabaseSet<TModel, TId> GetDbSet<TModel, TId>(string name)
            where TModel : IBaseModel<TId>;

        IDatabaseSet<TModel, TId> GetDbSet<TModel, TId>()
            where TModel : IBaseModel<TId>;
    }
}
