using ecestockroom_api.Helpers.Authentication;
using ecestockroom_api.Models.Authentication;
using ecestockroom_api.Models.Logging;
using ecestockroom_api.Services.Authentication;
using ecestockroom_api.Services.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ecestockroom_api.Controllers.Authentication
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly AuthenticationService _authenticationService;
        private readonly LogService _logService;
        private readonly RoleService _roleService;

        public RoleController(AuthenticationService authenticationService, LogService logService, RoleService roleService)
        {
            _authenticationService = authenticationService;
            _logService = logService;
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Role>>> Get([FromHeader] string authToken)
        {
            if (!await _authenticationService.CheckAccess(authToken, "roleView"))
            {
                return Unauthorized();
            }

            return await _roleService.Get();
        }

        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Role>> Get([FromHeader] string authToken, string id)
        {
            if (!await _authenticationService.CheckAccess(authToken, "roleView"))
            {
                return Unauthorized();
            }

            var role = await _roleService.Get(id);

            if (role == null)
            {
                return NotFound();
            }

            return role;
        }

        [HttpPost]
        public async Task<ActionResult<Role>> Create([FromHeader] string authToken, RoleCreate role)
        {
            if (!await _authenticationService.CheckAccess(authToken, "roleMgr"))
            {
                return Unauthorized();
            }

            Role created = await _roleService.Create(role);

            await _logService.Create(new Log(
                null,
                AuthenticationHelpers.GetUserIdFromToken(authToken),
                DateTime.UtcNow,
                "Document created.",
                "auth.roles",
                created.Id,
                JsonSerializer.Serialize(created)
            ));

            return Ok(role);
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update([FromHeader] string authToken, string id, RoleUpdate roleIn)
        {
            if (!await _authenticationService.CheckAccess(authToken, "roleMgr"))
            {
                return Unauthorized();
            }

            var role = await _roleService.Get(id);

            if (role == null)
            {
                return NotFound();
            }

            _roleService.Update(id, roleIn);

            await _logService.Create(new Log(
                null,
                AuthenticationHelpers.GetUserIdFromToken(authToken),
                DateTime.UtcNow,
                "Document modified.",
                "auth.roles",
                id,
                JsonSerializer.Serialize(Role.FromUpdate(id, roleIn))
            ));

            return Ok();
        }
    }
}
