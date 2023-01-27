using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;


namespace proyectoToken.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : ControllerBase
    {
        [Authorize]
        [HttpGet]
        [Route("List")]
        // This method just can be asked for the authorized users
        public async Task<IActionResult> List()
        {
            var countriesList = await Task.FromResult(new List<string> { "France", "Argentina", "Croatia", "Marocco" });
            return Ok(countriesList);
        }
    }
}
