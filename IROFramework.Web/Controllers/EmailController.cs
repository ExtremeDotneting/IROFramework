using System.Threading.Tasks;
using IRO.Mvc.CoolSwagger;
using IROFramework.Core.AppEnvironment;
using IROFramework.Core.Consts;
using IROFramework.Web.Dto.EmailDto;
using IROFramework.Web.Tools.Email;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IROFramework.Web.Controllers
{
    [ApiController]

    [SwaggerTagName("Email")]

    [Route(CommonConsts.ApiPath + "/email")]
    public class EmailController : ControllerBase
    {
        readonly IEmailService _emailService;

        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost("sendMessage")]
        public async Task SendMessage(SendEmailRequest req)
        {
            await _emailService.SendEmailAsync(
                req.Email,
                req.Subject,
                req.Text
            );
        }
    }
}