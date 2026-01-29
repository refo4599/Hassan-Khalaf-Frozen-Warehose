using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Frozen_Warehouse.Application.DTOs.Outbound;
using Frozen_Warehouse.Application.Interfaces.IServices;

namespace Frozen_Warehouse.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // TODO: Re-enable role restrictions after development
    public class OutboundController : ControllerBase
    {
        private readonly IOutboundService _outboundService;

        public OutboundController(IOutboundService outboundService)
        {
            _outboundService = outboundService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateOutboundRequest request)
        {
            try
            {
                var id = await _outboundService.CreateOutboundAsync(request);
                return CreatedAtAction(nameof(Create), new { id }, new { id });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
    }
}