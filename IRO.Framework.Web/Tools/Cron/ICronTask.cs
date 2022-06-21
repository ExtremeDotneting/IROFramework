using System.Threading.Tasks;

namespace IROFramework.Web.Tools.Cron
{
    public interface ICronTask
    {
        Task Handle();
    }
}