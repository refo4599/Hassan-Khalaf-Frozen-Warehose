using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Frozen_Warehouse.Application.DTOs.Inbound;
using Frozen_Warehouse.Application.Interfaces.IServices;

namespace Frozen_Warehouse.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize] // TODO: Re-enable role restrictions after development
    public class InboundController : ControllerBase
    {
        private readonly IInboundService _inboundService;

        public InboundController(IInboundService inboundService)
        {
            _inboundService = inboundService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateInboundRequest request)
        {
            try
            {
                var id = await _inboundService.CreateInboundAsync(request);
                return CreatedAtAction(nameof(Create), new { id }, new { id });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllInbound()
        {
            try
            {
                var inbounds = await _inboundService.GetAllInboundsAsync();
                return Ok(inbounds);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
        // Distinct route for daily report to avoid duplicate HTTP GET route conflicts
        [HttpGet("daily-report")]
        public async Task<IActionResult> GetDailyInboundReport()
        {
            try
            {
                var report = await _inboundService.GetDailyInboundReportAsync();
                return Ok(report);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
        [HttpGet("ReportFromTo")]
        public async Task<IActionResult> GetInboundReportFromTo(DateTime startDate, DateTime endDate)
        {
            try
            {
                var report = await _inboundService.GetInboundReportFromToAsync(startDate, endDate);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
        [HttpPost("update/{id}")]
        public async Task<IActionResult> UpdateInbound(int id, UpdateInboundRequest request)
        {
            try
            {
                await _inboundService.UpdateInboundAsync(id, request);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

    }
}