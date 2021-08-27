using ecestockroom_api.Helpers.Authentication;
using ecestockroom_api.Models.Authentication;
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
    public class PPSwipeController : ControllerBase
    {
        private readonly AuthenticationService _authenticationService;
        private readonly LogService _logService;
        private readonly PPEventService _ppEventService;
        private readonly UserService _userService;

        public PPSwipeController(AuthenticationService authenticationService, LogService logService, PPEventService ppEventService, UserService userService)
        {
            _authenticationService = authenticationService;
            _logService = logService;
            _ppEventService = ppEventService;
            _userService = userService;
        }

        [HttpGet("in/{techId:length(8)}/{eventId:length(24)}")]
        public async Task<ActionResult> SwipeIn([FromHeader] string authToken, string techId, string eventId)
        {
            if (!await _authenticationService.CheckAccess(authToken, "ppSwipe"))
            {
                return Unauthorized();
            }

            User user = await _userService.GetByTechId(techId);
            PPEvent ppEvent = await _ppEventService.Get(eventId);

            if (user == null)
            {
                user = await _userService.Create(new User(
                    null, 
                    techId,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    new List<PPUserEntry>()
                ));

                await _logService.Create(new Log(
                    null,
                    AuthenticationHelpers.GetUserIdFromToken(authToken),
                    DateTime.UtcNow,
                    "User created for Pro Point Event.",
                    "auth.users",
                    user.Id,
                    JsonSerializer.Serialize(user)
                ));
                //return NotFound("tech id not found");
            }

            if (ppEvent == null)
            {
                return NotFound("pro point event not found");
            }

            if(user.ProPoints.Find(entry => entry.EventId == eventId) != null)
            {
                return Problem("user is already checked in");
            }

            user.ProPoints.Add(new PPUserEntry(
                eventId,
                DateTime.UtcNow,
                null,
                null,
                "Active"
            ));

            _userService.Update(user.Id, user);

            await _logService.Create(new Log(
                null,
                AuthenticationHelpers.GetUserIdFromToken(authToken),
                DateTime.UtcNow,
                "User checked in for Pro Point Event.",
                "auth.users",
                user.Id,
                JsonSerializer.Serialize(user)
            ));

            return Ok();
        }

        [HttpGet("out/{techId:length(8)}/{eventId:length(24)}")]
        public async Task<ActionResult> SwipeOut([FromHeader] string authToken, string techId, string eventId)
        {
            if (!await _authenticationService.CheckAccess(authToken, "ppSwipe"))
            {
                return Unauthorized();
            }

            User user = await _userService.GetByTechId(techId);
            PPEvent ppEvent = await _ppEventService.Get(eventId);

            if (user == null)
            {
                return NotFound("tech id not found");
            }

            if (ppEvent == null)
            {
                return NotFound("pro point event not found");
            }

            PPUserEntry userEvent = user.ProPoints.Find(entry => entry.EventId == eventId);

            if (userEvent == null)
            {
                return Problem("user never checked in");
            }

            if (userEvent.CheckOutUtc != null)
            {
                return Problem("user already checked out");
            }

            DateTime now = DateTime.UtcNow;
            TimeSpan elapsed = now - userEvent.CheckInUtc;
            int hours = (int)Math.Round(elapsed.TotalHours);
            int points = hours * ppEvent.HourlyPointRate;

            userEvent.CheckOutUtc = now;
            userEvent.Points = points;

            _userService.Update(user.Id, user);

            await _logService.Create(new Log(
                null,
                AuthenticationHelpers.GetUserIdFromToken(authToken),
                DateTime.UtcNow,
                "User checked out for Pro Point Event.",
                "auth.users",
                user.Id,
                JsonSerializer.Serialize(user)
            ));

            return Ok();
        }
    }
}
