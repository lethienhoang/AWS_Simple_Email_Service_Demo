using Microsoft.AspNetCore.Mvc;

namespace Amazon.EmailService.Controllers
{
    [Route("healthcheck")]
    [ApiController]
    public class HealthCheckController : ControllerBase
    {
        [HttpGet]
        public ActionResult Get()
        {
            return Ok("Email Service is running.");
        }
    }
}
