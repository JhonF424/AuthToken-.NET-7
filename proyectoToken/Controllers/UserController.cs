using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using proyectoToken.Models.Custom;
using proyectoToken.Services;
    
namespace proyectoToken.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase {
        private readonly IAuthService _authService;

        public UserController(IAuthService authService) {
            _authService = authService;
        }

        [HttpPost]
        [Route("Authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] AuthRequest authroziation) {
            var authResult = await _authService.ReturnToken(authroziation);
            if (authResult == null) {
                return Unauthorized();
            }

            return Ok(authResult);
        }
    }
}
