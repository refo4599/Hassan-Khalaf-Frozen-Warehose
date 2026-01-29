using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Frozen_Warehouse.Application.Interfaces.IServices;

namespace Frozen_Warehouse.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // TODO: Re-enable role restrictions after development
    public class StockController : ControllerBase
    {
        private readonly IStockService _stockService;

        public StockController(IStockService stockService)
        {
            _stockService = stockService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int clientId, [FromQuery] int productId, [FromQuery] int sectionId)
        {
            var qty = await _stockService.GetStockAsync(clientId, productId, sectionId);
            return Ok(new { clientId, productId, sectionId, quantity = qty });
        }
    }
}