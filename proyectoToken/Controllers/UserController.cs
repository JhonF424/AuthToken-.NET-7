using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using proyectoToken.Models.Custom;
using proyectoToken.Services;
using System.IdentityModel.Tokens.Jwt;

namespace proyectoToken.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IAuthService _authService;

        public UserController( IAuthService authService )
        {
            _authService = authService;
        }

        [HttpPost]
        [Route("Authenticate")]
        public async Task<IActionResult> Authenticate( [FromBody] AuthRequest authroziation )
        {
            var authResult = await _authService.ReturnToken(authroziation);
            if (authResult == null)
            {
                return Unauthorized();
            }

            return Ok(authResult);
        }

        [HttpPost]
        [Route("ObtainRefreshToken")]
        public async Task<IActionResult> ObtainRefreshToken( [FromBody] RefreshTokenRequest request )
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var supposedExpiredToken = tokenHandler.ReadJwtToken(request.ExpiredToken);

            if (supposedExpiredToken.ValidTo > DateTime.UtcNow)
            {
                return BadRequest(new AuthResponse { Result = false, Message = "The current token is still available" });
            }

            string userId = supposedExpiredToken.Claims.First(x =>
            x.Type == JwtRegisteredClaimNames.NameId).Value.ToString();

            var authResponse = await _authService.ReturnRefreshToken(request, int.Parse(userId));

            if (authResponse.Result)
            {
                return Ok(authResponse);
            }
            else
            {
                return BadRequest(authResponse);
            }
        }
    }
}
