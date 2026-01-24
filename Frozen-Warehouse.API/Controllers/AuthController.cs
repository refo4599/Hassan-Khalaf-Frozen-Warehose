using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Frozen_Warehouse.Application.DTOs.Auth;
using Frozen_Warehouse.Application.Interfaces.IServices;

namespace Frozen_Warehouse.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var token = await _authService.AuthenticateAsync(request);
                return Ok(new { access_token = token });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }
    }
}