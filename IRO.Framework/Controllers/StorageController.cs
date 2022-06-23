using System.Collections.Generic;
using System.Threading.Tasks;
using IRO.Mvc.CoolSwagger;
using IRO.Storage;
using IROFramework.Core.AppEnvironment;
using IROFramework.Core.Models;
using IROFramework.Web.Tools.Email;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IROFramework.Web.Controllers
{
    [ApiController]
    [SwaggerTagName("Storage")]
    [Authorize]
    [Route(CommonConsts.ApiPath + "/storage")]
    public class StorageController : ControllerBase
    {
        readonly IKeyValueStorage _strorage;

        public StorageController(IKeyValueStorage strorage)
        {
            _strorage = strorage;
        }

        [HttpGet("getAll")]
        public async Task<IDictionary<string, string>> GetAll([FromQuery] string scopeName)
        {
            var dataModel = await _strorage.Get<DataModel>($"DataScope_{scopeName}");
            var dict = dataModel.DataDict;
            return dict;
        }

        [HttpPost("setAll")]
        public async Task SetAll([FromQuery] string scopeName, [FromBody]IDictionary<string, string> allData)
        {
            await _strorage.Set($"DataScope_{scopeName}", allData);
        }
    }
}
