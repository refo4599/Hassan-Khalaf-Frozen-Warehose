//using Microsoft.AspNetCore.Mvc;
//using Frozen_Warehouse.Application.Interfaces.IServices;
//using Frozen_Warehouse.Application.DTOs.Client;

//namespace Frozen_Warehouse.API.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class ClientsController : ControllerBase
//    {
//        private readonly IClientService _clientService;

//        public ClientsController(IClientService clientService)
//        {
//            _clientService = clientService;
//        }

//        [HttpGet]
//        public async Task<IActionResult> GetAll()
//        {
//            var clients = await _clientService.GetAllAsync();
//            return Ok(clients);
//        }

//        [HttpGet("{id:int}")]
//        public async Task<IActionResult> GetById(int id)
//        {
//            var client = await _clientService.GetByIdAsync(id);
//            if (client == null) return NotFound();
//            return Ok(client);
//        }

//        [HttpPost]
//        public async Task<IActionResult> Create([FromBody] CreateClientRequest request)
//        {
//            if (!ModelState.IsValid) return BadRequest(ModelState);
//            try
//            {
//                var created = await _clientService.CreateAsync(request);
//                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
//            }
//            catch (ArgumentException ex)
//            {
//                return Conflict(new { message = ex.Message });
//            }
//        }

//        [HttpPut("{id:int}")]
//        public async Task<IActionResult> Update(int id, [FromBody] UpdateClientRequest request)
//        {
//            if (!ModelState.IsValid) return BadRequest(ModelState);
//            try
//            {
//                var updated = await _clientService.UpdateAsync(id, request);
//                if (updated == null) return NotFound();
//                return Ok(updated);
//            }
//            catch (ArgumentException ex)
//            {
//                return Conflict(new { message = ex.Message });
//            }
//        }

//        [HttpDelete("{id:int}")]
//        public async Task<IActionResult> Delete(int id)
//        {
//            var deleted = await _clientService.DeleteAsync(id);
//            if (!deleted) return NotFound();
//            return NoContent();
//        }
//    }
//}
