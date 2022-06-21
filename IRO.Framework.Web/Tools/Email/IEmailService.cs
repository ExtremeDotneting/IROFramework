using System.Threading.Tasks;

namespace IROFramework.Web.Tools.Email
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string text);
        void Dispose();
    }
}