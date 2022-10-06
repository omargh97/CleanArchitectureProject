using Microsoft.AspNetCore.Mvc;

using Demo.Entities;
using Demo.IService;
using Demo.ViewModel;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Http;
using Demo.WebAPI.Infrastructure.JwtAuth;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using Demo.WebAPI.Middleware;
using Demo.WebAPI.Helper;
using AutoMapper;
using System.Security.Cryptography;
using System.Text;

namespace Demo.WebAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/Authentication")]
    [ApiVersion("1.0")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        public readonly IService.IAuthenticationService _authenticationService;
        public readonly IUserService _userService;
        public readonly IJwtAuthManager _jwtAuthManager;
        private readonly IMapper _mapper;

        public AuthenticationController(IService.IAuthenticationService authenticationService, IUserService userService, IJwtAuthManager jwtAuthManager,IMapper mapper)
        {
            _authenticationService = authenticationService;
            _userService = userService;
            _jwtAuthManager = jwtAuthManager;
            _mapper = mapper;
        }

        // POST: api/User
        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequestVM loginVM)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Users _user = await _authenticationService.Login(loginVM.UserName, loginVM.Password);

            if (_user == null)
            {
                return Unauthorized(new { message = "Username is incorrect" });
            }

            using var hmac = new HMACSHA512(_user.PasswordSalt);

            var computedhash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginVM.Password));

            for (int i = 0; i < computedhash.Length; i++)
            {
                if (computedhash[i] != _user.PasswordHash[i]) return Unauthorized("Password is incorrect");
            }

            var claims = new[]
           {
                new Claim("Name",_user.UserName),
                new Claim("Role",_user.Role.ToString()),
                new Claim("Id",_user.Id.ToString())
            };

            var jwtResult = _jwtAuthManager.GenerateTokens(_user.UserName, claims, DateTime.Now);

            return Ok(new LoginResultVM
            {
                UserName = _user.UserName,
                Role = null,
                AccessToken = jwtResult.AccessToken,
                RefreshToken = jwtResult.RefreshToken.TokenString,
                OriginalUserName = _user.FirstName + " " + _user.LastName
            });
        }

        [HttpGet("user")]
        [Authorize]
        public IActionResult GetCurrentUser()
        {
            return Ok(new LoginResultVM
            {
                UserName = User.Identity.Name,
                Role = User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty,
                OriginalUserName = User.FindFirst("OriginalUserName")?.Value
            });
        }

        [HttpPost("logout")]
        [Authorize]
        public ActionResult Logout()
        {
            // optionally "revoke" JWT token on the server side --> add the current token to a block-list
            // https://github.com/auth0/node-jsonwebtoken/issues/375

            var userName = User.Identity.Name;
            _jwtAuthManager.RemoveRefreshTokenByUserName(userName);
            var env = Request.Headers;

            return Ok();
        }

        [HttpPost("refresh-token")]
        [Authorize]
        public async Task<ActionResult> RefreshToken([FromBody] RefreshTokenRequestVM request)
        {
            try
            {
                var userName = User.Identity.Name;

                if (string.IsNullOrWhiteSpace(request.RefreshToken))
                {
                    return Unauthorized();
                }

                var accessToken = await HttpContext.GetTokenAsync("Bearer", "access_token");
                var jwtResult = _jwtAuthManager.Refresh(request.RefreshToken, accessToken, DateTime.Now);
                return Ok(new LoginResultVM
                {
                    UserName = userName,
                    Role = User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty,
                    AccessToken = jwtResult.AccessToken,
                    RefreshToken = jwtResult.RefreshToken.TokenString
                });
            }
            catch (SecurityTokenException e)
            {
                return Unauthorized(e.Message); // return 401 so that the client side can redirect the user to login page
            }
        }

        [HttpPost("Signin")]
        [AllowAnonymous]
        public async Task<IActionResult> Signin(SigninVM user)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "The request is invalid", ModelState });
                }

                using var hmac = new HMACSHA512();

                Users MappingUser = new Users
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserName = user.UserName,
                    PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(user.Password)),
                    PasswordSalt = hmac.Key
                };

                bool status = await _userService.Add(MappingUser);

                if (status == false)
                {
                    return BadRequest(new { message = "Username already taken" });
                }
                return StatusCode(StatusCodes.Status201Created, new { message = "User creadted" });
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, exp.Message);
            }
        }
    }
}

