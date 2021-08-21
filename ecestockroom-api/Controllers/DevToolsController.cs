using ecestockroom_api.Helpers.Authentication;
using Microsoft.AspNetCore.Mvc;
using System;

namespace ecestockroom_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DevToolsController : ControllerBase
    {
        [HttpGet("hashpassword")]
        public IActionResult HashPassword([FromQuery] string password)
        {
            return Ok(AuthenticationHelpers.EncrpytPassword(password));
        }

        [HttpGet("verifypassword")]
        public IActionResult VerifyPassword([FromQuery] string password, [FromQuery] string hashedPassword)
        {
            return Ok(AuthenticationHelpers.IsPasswordValid(password, hashedPassword));
        }
    }
}
