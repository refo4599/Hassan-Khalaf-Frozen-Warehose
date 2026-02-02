using Frozen_Warehouse.Domain.Entities;
using Frozen_Warehouse.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;

namespace Frozen_Warehouse.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ClientController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAllClient()
        {
            var clients = _context.Clients;
            return Ok(clients);
        }

        [HttpGet("by-id/{id}")]
        public IActionResult GetClientById(int id)
        {
            var client = _context.Clients.Find(id);
            if (client == null)
            {
                return NotFound();
            }
            return Ok(client);
        }
        [HttpGet("by-name/{name}")]
        public IActionResult GetClientByName(string name)
        {
            var client = _context.Clients.FirstOrDefault(c => c.Name == name);
            if (client == null)
            {
                return NotFound();
            }
            return Ok(client);
        }
        [HttpPost]
        public IActionResult CreateClient(Client client)
        {
            if (client == null)
            {
                return BadRequest("client can not be null");
            }
            _context.Clients.AddAsync(client);
            _context.SaveChanges();
            return Ok();
        }
        [HttpPut]
        public IActionResult UpdateClient(int id, Client client)
        {
            var exitingclient = _context.Clients.Find(id);
            if (exitingclient == null)
            {
                return BadRequest("client not found");
            }
            exitingclient.Name = client.Name;
            _context.SaveChanges();
            return Ok();
        }
    } 
}
