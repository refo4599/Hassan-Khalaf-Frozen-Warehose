using Microsoft.AspNetCore.Mvc;
using Frozen_Warehouse.Application.Interfaces.IServices;
using Frozen_Warehouse.Application.DTOs.Section;

namespace Frozen_Warehouse.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SectionsController : ControllerBase
    {
        private readonly ISectionService _sectionService;

        public SectionsController(ISectionService sectionService)
        {
            _sectionService = sectionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var sections = await _sectionService.GetAllAsync();
            return Ok(sections);
        }
    }
}
