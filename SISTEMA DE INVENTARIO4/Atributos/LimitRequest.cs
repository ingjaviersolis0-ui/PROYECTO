using Microsoft.AspNetCore.Mvc;
using SISTEMA_DE_INVENTARIO4.Attributes;
using Microsoft.AspNetCore.Authorization;
namespace SISTEMA_DE_INVENTARIO4.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "admin")]
    public class TestController : ControllerBase
    {
        [HttpGet("limitado")]
        [SimpleThrottle(MaxRequests = 3, Seconds = 15)]
        public IActionResult GetLimitado()
        {
            return Ok($"✅ OK - {DateTime.Now:HH:mm:ss}");
        }

        [HttpGet("libre")]
        public IActionResult GetLibre()
        {
            return Ok($"🆓 Sin límite - {DateTime.Now:HH:mm:ss}");
        }
    }
}