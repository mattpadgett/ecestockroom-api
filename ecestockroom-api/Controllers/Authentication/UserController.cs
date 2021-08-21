using ecestockroom_api.Helpers.Authentication;
using ecestockroom_api.Models.Authentication;
using ecestockroom_api.Models.Logging;
using ecestockroom_api.Services.Authentication;
using ecestockroom_api.Services.Logging;
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
    public class UserController : ControllerBase
    {
        private readonly AuthenticationService _authenticationService;
        private readonly LogService _logService;
        private readonly UserService _userService;
        private readonly RoleService _roleService;
        private readonly PermissionService _permissionService;
        private readonly TokenService _tokenService;

        public UserController(AuthenticationService authenticationService, LogService logService, UserService userService, RoleService roleService, PermissionService permissionService, TokenService tokenService)
        {
            _authenticationService = authenticationService;
            _logService = logService;
            _userService = userService;
            _roleService = roleService;
            _permissionService = permissionService;
            _tokenService = tokenService;
        }

        [HttpGet]
        public async Task<ActionResult<List<User>>> Get([FromHeader] string authToken)
        {
            if (!await _authenticationService.CheckAccess(authToken, "userView"))
            {
                return Unauthorized();
            }

            return await _userService.Get();
        }

        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<User>> Get([FromHeader] string authToken, string id)
        {
            if (!await _authenticationService.CheckAccess(authToken, "userView"))
            {
                return Unauthorized();
            }

            var user = await _userService.Get(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        [HttpPost]
        public async Task<ActionResult<User>> Create([FromHeader] string authToken, UserCreate user)
        {
            if (!await _authenticationService.CheckAccess(authToken, "userMgr"))
            {
                return Unauthorized();
            }

            User created = await _userService.Create(user);

            await _logService.Create(new Log(
                null,
                AuthenticationHelpers.GetUserIdFromToken(authToken),
                DateTime.UtcNow,
                "Document created.",
                "auth.users",
                created.Id,
                JsonSerializer.Serialize(created)
            ));

            return Ok(user);
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update([FromHeader] string authToken, string id, UserUpdate userIn)
        {
            if (!await _authenticationService.CheckAccess(authToken, "userMgr"))
            {
                return Unauthorized();
            }

            var user = await _userService.Get(id);

            if (user == null)
            {
                return NotFound();
            }

            _userService.Update(user, userIn);

            await _logService.Create(new Log(
                null,
                AuthenticationHelpers.GetUserIdFromToken(authToken),
                DateTime.UtcNow,
                "Document modified.",
                "auth.users",
                id,
                JsonSerializer.Serialize(ecestockroom_api.Models.Authentication.User.FromUpdate(user, userIn))
            ));

            return Ok();
        }

        [HttpGet("login")]
        public async Task<IActionResult> Login([FromQuery] string username, [FromQuery] string password)
        {
            User user = await _userService.GetByUsername(username);

            if (user == null)
            {
                return NotFound("username does not exist");
            }

            if (!AuthenticationHelpers.IsPasswordValid(password, user.Password))
            {
                return Unauthorized("incorrect password");
            }

            // Check if user has access to login
            List<Role> userRoles = new List<Role>();

            foreach (string roleId in user.Roles)
            {
                userRoles.Add(await _roleService.Get(roleId));
            }

            if (!AuthenticationHelpers.IsPermissionGranted(user, userRoles, Startup.StaticConfiguration.GetSection("PermissionIds")["login"]))
            {
                return Unauthorized("not authorized for login");
            }

            string authToken = AuthenticationHelpers.GenerateAuthToken(user, await _roleService.Get(), await _permissionService.Get());

            await _tokenService.Create(new Token(
                null,
                user.Id,
                "auth",
                authToken,
                DateTime.UtcNow,
                new List<TokenAction>(),
                false,
                true
            ));

            var createdAuthToken = await _tokenService.GetByToken(authToken);

            string refreshToken = AuthenticationHelpers.GenerateRefreshToken(user, createdAuthToken.Id);

            await _tokenService.Create(new Token(
                null,
                user.Id,
                "refresh",
                refreshToken,
                DateTime.UtcNow,
                new List<TokenAction>(),
                false,
                true
            ));

            return Ok(
                new Dictionary<string, string>
                {
                    { "authToken", authToken },
                    { "refreshToken", refreshToken }
                }
            );
        }

        [HttpGet("refresh")]
        public async Task<IActionResult> RefreshToken([FromQuery] string refreshToken)
        {
            Token token = await _tokenService.GetByToken(refreshToken);

            if (token == null)
            {
                return NotFound("refresh token not found");
            }

            // Check if token is valid in MongoDB
            if (!token.ValidFlag)
            {
                return Problem("invalidated token");
            }

            if (token.UsedFlag)
            {
                await _tokenService.InvalidateUserTokens(token.UserId);
                return Problem("refresh token already used");
            }

            // Check token validity (Expiration, issuer, audience, etc.)
            if(!AuthenticationHelpers.IsTokenValid(refreshToken))
            {
                return Problem("token failed validation");
            }

            var decodedToken = AuthenticationHelpers.ReadToken(refreshToken);

            await _tokenService.InvalidateToken(decodedToken.Claims.First(c => c.Type == "authTokenId").Value);
            await _tokenService.UseToken(token.Id);
            await _tokenService.AddUsage(token.Id, new TokenAction(DateTime.UtcNow, "Used refresh token to generate new pair."));

            // Generate new token pair
            var user = await _userService.Get(token.UserId);

            string authToken = AuthenticationHelpers.GenerateAuthToken(user, await _roleService.Get(), await _permissionService.Get());

            await _tokenService.Create(new Token(
                null,
                user.Id,
                "auth",
                authToken,
                DateTime.UtcNow,
                new List<TokenAction>(),
                false,
                true
            ));

            var createdAuthToken = await _tokenService.GetByToken(authToken);

            string newRefreshToken = AuthenticationHelpers.GenerateRefreshToken(user, createdAuthToken.Id);

            await _tokenService.Create(new Token(
                null,
                user.Id,
                "refresh",
                newRefreshToken,
                DateTime.UtcNow,
                new List<TokenAction>(),
                false,
                true
            ));

            return Ok(
                new Dictionary<string, string>
                {
                    { "authToken", authToken },
                    { "refreshToken", newRefreshToken }
                }
            );
        }
    }
}
