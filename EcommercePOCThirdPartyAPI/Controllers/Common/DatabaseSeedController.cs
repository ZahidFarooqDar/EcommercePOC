using EcommercePOCThirdPartyAPI.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EcommercePOCThirdPartyAPI.Controllers.Common
{
    [Route("api/[controller]")]
    [ApiController]
    public class DatabaseSeedController : ControllerBase
    {
        private readonly ProjectEcommerceContext _apiDbContext;
        //private readonly IPasswordEncryptHelper _passwordEncryptHelper;
        public DatabaseSeedController(ProjectEcommerceContext context)
        {
            _apiDbContext = context;
        }
        [HttpGet]
        [Route("Init")]
        public async Task<IActionResult> Get()
        {
            DatabaseSeeder<ProjectEcommerceContext> databaseSeeder = new DatabaseSeeder<ProjectEcommerceContext>();
            var retVal = await databaseSeeder.SetupDatabaseWithTestData(_apiDbContext);//, (x) => _passwordEncryptHelper.ProtectAsync<string>(x).Result);
            return Ok(retVal);
        }
    }
}
