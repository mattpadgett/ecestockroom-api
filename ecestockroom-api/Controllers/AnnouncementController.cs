using ecestockroom_api.Helpers.Authentication;
using ecestockroom_api.Models;
using ecestockroom_api.Models.Logging;
using ecestockroom_api.Services;
using ecestockroom_api.Services.Authentication;
using ecestockroom_api.Services.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ecestockroom_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnnouncementController : ControllerBase
    {
        private readonly AuthenticationService _authenticationService;
        private readonly LogService _logService;
        private readonly AnnouncementService _announcementService;

        public AnnouncementController(AuthenticationService authenticationService, LogService logService, AnnouncementService announcementService)
        {
            _authenticationService = authenticationService;
            _logService = logService;
            _announcementService = announcementService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Announcement>>> Get([FromHeader] string authToken)
        {
            if (!await _authenticationService.CheckAccess(authToken, "announceView"))
            {
                return Unauthorized();
            }

            List<Announcement> announcements = await _announcementService.Get();

            if (!await _authenticationService.CheckAccess(authToken, "announceMgr"))
            {
                List<Announcement> activeAnnouncements = new List<Announcement>();

                foreach (Announcement announcement in announcements)
                {
                    if (announcement.Status != "Active")
                        continue;

                    activeAnnouncements.Add(announcement);
                }

                return activeAnnouncements;
            }

            return announcements;
        }

        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Announcement>> Get([FromHeader] string authToken, string id)
        {
            if (!await _authenticationService.CheckAccess(authToken, "announceView"))
            {
                return Unauthorized();
            }

            var announcement = await _announcementService.Get(id);

            if (announcement == null)
            {
                return NotFound();
            }

            if (announcement.Status != "Active" && !await _authenticationService.CheckAccess(authToken, "announceMgr"))
            {
                return Unauthorized("you do not have the required permissions to view this announcement");
            }

            return announcement;
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete([FromHeader] string authToken, string id)
        {
            if (!await _authenticationService.CheckAccess(authToken, "announceMgr"))
            {
                return Unauthorized();
            }

            var announcement = await _announcementService.Get(id);

            if (announcement == null)
            {
                return NotFound();
            }

            await _announcementService.Delete(id);

            await _logService.Create(new Log(
                null,
                AuthenticationHelpers.GetUserIdFromToken(authToken),
                DateTime.UtcNow,
                "Document deleted.",
                "announcements",
                id,
                null
            ));

            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult<Announcement>> Create([FromHeader] string authToken, AnnouncementCreate announcement)
        {
            if (!await _authenticationService.CheckAccess(authToken, "announceMgr"))
            {
                return Unauthorized();
            }

            Announcement created = await _announcementService.Create(announcement);

            await _logService.Create(new Log(
                null,
                AuthenticationHelpers.GetUserIdFromToken(authToken),
                DateTime.UtcNow,
                "Document created.",
                "announcements",
                created.Id,
                JsonSerializer.Serialize(created)
            ));

            return Ok(announcement);
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update([FromHeader] string authToken, string id, AnnouncementUpdate announcementIn)
        {
            if (!await _authenticationService.CheckAccess(authToken, "announceMgr"))
            {
                return Unauthorized();
            }

            var announcement = await _announcementService.Get(id);

            if (announcement == null)
            {
                return NotFound();
            }

            _announcementService.Update(announcement, announcementIn);

            await _logService.Create(new Log(
                null,
                AuthenticationHelpers.GetUserIdFromToken(authToken),
                DateTime.UtcNow,
                "Document modified.",
                "announcements",
                id,
                JsonSerializer.Serialize(Announcement.FromUpdate(announcement, announcementIn))
            ));

            return Ok();
        }
    }
}
