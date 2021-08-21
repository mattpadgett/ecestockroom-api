using ecestockroom_api.Models.Authentication;
using ecestockroom_api.Services.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ecestockroom_api.Controllers.Authentication
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        private readonly AuthenticationService _authenticationService;
        private readonly PermissionService _permissionService;

        public PermissionController(AuthenticationService authenticationService, PermissionService permissionService)
        {
            _authenticationService = authenticationService;
            _permissionService = permissionService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Permission>>> Get([FromHeader] string authToken)
        {
            if (!await _authenticationService.CheckAccess(authToken, "permView"))
            {
                return Unauthorized();
            }

            return await _permissionService.Get();
        }

        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Permission>> Get([FromHeader] string authToken, string id)
        {
            if (!await _authenticationService.CheckAccess(authToken, "permView"))
            {
                return Unauthorized();
            }

            var permission = await _permissionService.Get(id);

            if (permission == null)
            {
                return NotFound();
            }

            return permission;
        }
    }
}
