using ecestockroom_api.Helpers.Authentication;
using ecestockroom_api.Models.Logging;
using ecestockroom_api.Models.ProPoints;
using ecestockroom_api.Services.Authentication;
using ecestockroom_api.Services.Logging;
using ecestockroom_api.Services.ProPoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ecestockroom_api.Controllers.ProPoints
{
    [Route("api/[controller]")]
    [ApiController]
    public class PPEventController : ControllerBase
    {
        private readonly AuthenticationService _authenticationService;
        private readonly LogService _logService;
        private readonly PPEventService _ppEventService;

        public PPEventController(AuthenticationService authenticationService, LogService logService, PPEventService ppEventService)
        {
            _authenticationService = authenticationService;
            _logService = logService;
            _ppEventService = ppEventService;
        }

        [HttpGet]
        public async Task<ActionResult<List<PPEvent>>> Get([FromHeader] string authToken)
        {
            if (!await _authenticationService.CheckAccess(authToken, "ppEventView"))
            {
                return Unauthorized();
            }

            List<PPEvent> ppEvents = await _ppEventService.Get();

            if (!await _authenticationService.CheckAccess(authToken, "ppEventMgr"))
            {
                List<PPEvent> activeEvents = new List<PPEvent>();

                foreach (PPEvent ppEvent in ppEvents)
                {
                    if (ppEvent.Status != "Active")
                        continue;

                    activeEvents.Add(ppEvent);
                }

                return activeEvents;
            }

            return ppEvents;
        }

        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<PPEvent>> Get([FromHeader] string authToken, string id)
        {
            if (!await _authenticationService.CheckAccess(authToken, "ppEventView"))
            {
                return Unauthorized();
            }

            var ppEvent = await _ppEventService.Get(id);

            if (ppEvent == null)
            {
                return NotFound();
            }

            if (ppEvent.Status != "Active" && !await _authenticationService.CheckAccess(authToken, "ppEventMgr"))
            {
                return Unauthorized("you do not have the required permissions to view this pro points event");
            }

            return ppEvent;
        }

        [HttpGet("daily")]
        public async Task<ActionResult<List<PPEvent>>> GetTodaysEvents([FromHeader] string authToken)
        {
            if (!await _authenticationService.CheckAccess(authToken, "ppSwipe") && !await _authenticationService.CheckAccess(authToken, "ppEventView"))
            {
                return Unauthorized();
            }

            List<PPEvent> ppEvents = await _ppEventService.Get();
            List<PPEvent> todaysEvents = new List<PPEvent>();

            foreach(PPEvent ppEvent in ppEvents)
            {
                if(ppEvent.Status == "Active" && ppEvent.EventUtc.ToLocalTime().Date == DateTime.Today)
                {
                    todaysEvents.Add(ppEvent);
                }
            }

            return todaysEvents;
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete([FromHeader] string authToken, string id)
        {
            if (!await _authenticationService.CheckAccess(authToken, "ppEventMgr"))
            {
                return Unauthorized();
            }

            var ppEvent = await _ppEventService.Get(id);

            if (ppEvent == null)
            {
                return NotFound();
            }

            await _ppEventService.Delete(id);

            await _logService.Create(new Log(
                null,
                AuthenticationHelpers.GetUserIdFromToken(authToken),
                DateTime.UtcNow,
                "Document deleted.",
                "pp.events",
                id,
                null
            ));

            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult<PPEvent>> Create([FromHeader] string authToken, PPEventCreate create)
        {
            if (!await _authenticationService.CheckAccess(authToken, "ppEventMgr"))
            {
                return Unauthorized();
            }

            PPEvent created = await _ppEventService.Create(create);

            await _logService.Create(new Log(
                null,
                AuthenticationHelpers.GetUserIdFromToken(authToken),
                DateTime.UtcNow,
                "Document created.",
                "pp.events",
                created.Id,
                JsonSerializer.Serialize(created)
            ));

            return Ok(create);
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update([FromHeader] string authToken, string id, PPEventUpdate update)
        {
            if (!await _authenticationService.CheckAccess(authToken, "ppEventMgr"))
            {
                return Unauthorized();
            }

            var ppEvent = await _ppEventService.Get(id);

            if (ppEvent == null)
            {
                return NotFound();
            }

            _ppEventService.Update(ppEvent, update);

            await _logService.Create(new Log(
                null,
                AuthenticationHelpers.GetUserIdFromToken(authToken),
                DateTime.UtcNow,
                "Document modified.",
                "pp.events",
                id,
                JsonSerializer.Serialize(PPEvent.FromUpdate(ppEvent, update))
            ));

            return Ok();
        }
    }
}
