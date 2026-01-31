using Frozen_Warehouse.Domain.Entities;
using Frozen_Warehouse.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;

namespace Frozen_Warehouse.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SectionController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;
        private readonly ApplicationDbContext _context;
        public SectionController(ILogger<ProductController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        [HttpGet]
        public IActionResult GetAllSections()
        {
            var sections = _context.Sections.ToList();
            return Ok(sections);
        }
        [HttpPost]
        public IActionResult AddSection(Section section)
        {
            if (section == null)
            {
                throw new ArgumentNullException("section");
            }
            _context.Sections.AddAsync(section);
            _context.SaveChanges();
            return Ok();
        }
    }
}
