namespace IROFramework.Core.Tools.AbstractDatabase
{
    public interface IBaseModel<TId>
    {
        TId Id { get; set; }
    }
}