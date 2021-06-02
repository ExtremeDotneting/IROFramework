namespace IROFramework.Core.AppEnvironment
{
    public interface IEnvLoader
    {
        T GetValue<T>(string propName);
    }
}