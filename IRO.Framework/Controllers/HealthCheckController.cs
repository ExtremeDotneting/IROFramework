using System.Threading.Tasks;
using IRO.Mvc.CoolSwagger;
using Microsoft.AspNetCore.Mvc;

namespace IROFramework.Web.Controllers
{
    [ApiController]
    [SwaggerTagName("HealthCheck")]
    [Route("health")]
    public class HealthCheckController : ControllerBase
    {
        [HttpGet]
        public async Task<JsonResult> Get()
        {
            return new JsonResult("Server is launched.");
        }

        [HttpPost]
        public async Task<JsonResult> Post()
        {
            return new JsonResult("Server is launched.");
        }
    }
}