using ecestockroom_api.Helpers.Authentication;
using ecestockroom_api.Models.Logging;
using ecestockroom_api.Models.Tools;
using ecestockroom_api.Services.Authentication;
using ecestockroom_api.Services.Logging;
using ecestockroom_api.Services.Tools;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ecestockroom_api.Controllers.Tools
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToolController : ControllerBase
    {
        private readonly AuthenticationService _authenticationService;
        private readonly LogService _logService;
        private readonly ToolService _toolService;

        public ToolController(AuthenticationService authenticationService, LogService logService, ToolService toolService)
        {
            _authenticationService = authenticationService;
            _logService = logService;
            _toolService = toolService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ToolGroup>>> Get([FromHeader] string authToken)
        {
            if (!await _authenticationService.CheckAccess(authToken, "stuToolView"))
            {
                return Unauthorized();
            }

            List<ToolGroup> toolGroups = await _toolService.Get();

            if (!await _authenticationService.CheckAccess(authToken, "stuToolMgr"))
            {
                List<ToolGroup> activeToolGroups = new List<ToolGroup>();

                foreach(ToolGroup toolGroup in toolGroups)
                {
                    if (toolGroup.Status != "Active")
                        continue;

                    List<Tool> activeTools = new List<Tool>();

                    foreach(Tool tool in toolGroup.Tools)
                    {
                        if (tool.Status != "Active")
                            continue;

                        activeTools.Add(tool);
                    }

                    toolGroup.Tools = activeTools;

                    activeToolGroups.Add(toolGroup);
                }

                return activeToolGroups;
            }

            return toolGroups;
        }

        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<ToolGroup>> Get([FromHeader] string authToken, string id)
        {
            if (!await _authenticationService.CheckAccess(authToken, "stuToolView"))
            {
                return Unauthorized();
            }

            var toolGroup = await _toolService.Get(id);

            if (toolGroup == null)
            {
                return NotFound();
            }

            return toolGroup;
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete([FromHeader] string authToken, string id)
        {
            if (!await _authenticationService.CheckAccess(authToken, "stuToolMgr"))
            {
                return Unauthorized();
            }

            var toolGroup = await _toolService.Get(id);

            if (toolGroup == null)
            {
                return NotFound();
            }

            await _toolService.Delete(id);

            await _logService.Create(new Log(
                null,
                AuthenticationHelpers.GetUserIdFromToken(authToken),
                DateTime.UtcNow,
                "Document deleted.",
                "toolGroups",
                id,
                null
            ));

            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult<ToolGroup>> Create([FromHeader] string authToken, ToolGroupCreate tool)
        {
            if (!await _authenticationService.CheckAccess(authToken, "stuToolMgr"))
            {
                return Unauthorized();
            }

            ToolGroup created = await _toolService.Create(tool);

            await _logService.Create(new Log(
                null,
                AuthenticationHelpers.GetUserIdFromToken(authToken),
                DateTime.UtcNow,
                "Document created.",
                "toolGroups",
                created.Id,
                JsonSerializer.Serialize(created)
            ));

            return Ok(tool);
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update([FromHeader] string authToken, string id, ToolGroupUpdate groupIn)
        {
            if (!await _authenticationService.CheckAccess(authToken, "stuToolMgr"))
            {
                return Unauthorized();
            }

            var toolGroup = await _toolService.Get(id);

            if (toolGroup == null)
            {
                return NotFound();
            }

            _toolService.Update(toolGroup, groupIn);

            await _logService.Create(new Log(
                null,
                AuthenticationHelpers.GetUserIdFromToken(authToken),
                DateTime.UtcNow,
                "Document modified.",
                "toolGroups",
                id,
                JsonSerializer.Serialize(ToolGroup.FromUpdate(toolGroup, groupIn))
            ));

            return Ok();
        }
    }
}
