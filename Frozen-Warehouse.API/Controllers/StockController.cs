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
        public async Task<IActionResult> Get([FromQuery] Guid clientId, [FromQuery] Guid productId, [FromQuery] Guid sectionId)
        {
            var stock = await _stockService.GetStockAsync(clientId, productId, sectionId);
            return Ok(stock);
        }
    }
}