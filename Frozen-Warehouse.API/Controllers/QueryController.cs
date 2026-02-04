using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Frozen_Warehouse.Infrastructure.Data;
using Frozen_Warehouse.Application.DTOs.Stock;

namespace Frozen_Warehouse.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QueryController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public QueryController(ApplicationDbContext db)
        {
            _db = db;
        }

        private sealed record AggregateDto(int Id, int TotalCartons, int TotalPallets);
        private sealed record ClientSectionDto(int ClientId, int SectionId, int TotalCartons, int TotalPallets);

        /// <summary>
        /// 1) Total stock for a section (all clients & products)
        /// GET api/query/section/{sectionId}/total
        /// </summary>
        [HttpGet("section/{sectionId}/total")]
        public async Task<IActionResult> SectionTotal([FromRoute] int sectionId)
        {
            var exists = await _db.Sections.AnyAsync(s => s.Id == sectionId);
            if (!exists) return NotFound(new { Message = $"Section {sectionId} not found." });

            var inboundCartons = await _db.InboundDetails.Where(d => d.SectionId == sectionId).SumAsync(d => (int?)d.Cartons) ?? 0;
            var inboundPallets = await _db.InboundDetails.Where(d => d.SectionId == sectionId).SumAsync(d => (int?)d.Pallets) ?? 0;

            var outboundCartons = await _db.OutboundDetails.Where(d => d.SectionId == sectionId).SumAsync(d => (int?)d.Cartons) ?? 0;
            var outboundPallets = await _db.OutboundDetails.Where(d => d.SectionId == sectionId).SumAsync(d => (int?)d.Pallets) ?? 0;

            var totalCartons = inboundCartons - outboundCartons;
            var totalPallets = inboundPallets - outboundPallets;

            var aggregate = new AggregateDto(sectionId, totalCartons, totalPallets);
            return Ok(aggregate);
        }

        /// <summary>
        /// 2) Total stock for a specific client in a specific section.
        /// GET api/query/section/{sectionId}/client/{clientId}
        /// </summary>
        [HttpGet("section/{sectionId}/client/{clientId}")]
        public async Task<IActionResult> ClientInSection([FromRoute] int sectionId, [FromRoute] int clientId)
        {
            var sectionExists = await _db.Sections.AnyAsync(s => s.Id == sectionId);
            if (!sectionExists) return NotFound(new { Message = $"Section {sectionId} not found." });

            var clientExists = await _db.Clients.AnyAsync(c => c.Id == clientId);
            if (!clientExists) return NotFound(new { Message = $"Client {clientId} not found." });

            var inboundCartons = await (from d in _db.InboundDetails
                                        join i in _db.Inbounds on d.InboundId equals i.Id
                                        where d.SectionId == sectionId && i.ClientId == clientId
                                        select (int?)d.Cartons).SumAsync() ?? 0;

            var inboundPallets = await (from d in _db.InboundDetails
                                        join i in _db.Inbounds on d.InboundId equals i.Id
                                        where d.SectionId == sectionId && i.ClientId == clientId
                                        select (int?)d.Pallets).SumAsync() ?? 0;

            var outboundCartons = await (from d in _db.OutboundDetails
                                         join o in _db.Outbounds on d.OutboundId equals o.Id
                                         where d.SectionId == sectionId && o.ClientId == clientId
                                         select (int?)d.Cartons).SumAsync() ?? 0;

            var outboundPallets = await (from d in _db.OutboundDetails
                                         join o in _db.Outbounds on d.OutboundId equals o.Id
                                         where d.SectionId == sectionId && o.ClientId == clientId
                                         select (int?)d.Pallets).SumAsync() ?? 0;

            var totalCartons = inboundCartons - outboundCartons;
            var totalPallets = inboundPallets - outboundPallets;

            var aggregate = new ClientSectionDto(clientId, sectionId, totalCartons, totalPallets);
            return Ok(aggregate);
        }

        /// <summary>
        /// 3) Stock for a specific client & product in a specific section.
        /// GET api/query/section/{sectionId}/client/{clientId}/product/{productId}
        /// </summary>
        [HttpGet("section/{sectionId}/client/{clientId}/product/{productId}")]
        public async Task<IActionResult> ClientProductInSection([FromRoute] int sectionId, [FromRoute] int clientId, [FromRoute] int productId)
        {
            var sectionExists = await _db.Sections.AnyAsync(s => s.Id == sectionId);
            if (!sectionExists) return NotFound(new { Message = $"Section {sectionId} not found." });

            var clientExists = await _db.Clients.AnyAsync(c => c.Id == clientId);
            if (!clientExists) return NotFound(new { Message = $"Client {clientId} not found." });

            var productExists = await _db.Products.AnyAsync(p => p.Id == productId);
            if (!productExists) return NotFound(new { Message = $"Product {productId} not found." });

            var inboundCartons = await (from d in _db.InboundDetails
                                        join i in _db.Inbounds on d.InboundId equals i.Id
                                        where d.SectionId == sectionId && i.ClientId == clientId && d.ProductId == productId
                                        select (int?)d.Cartons).SumAsync() ?? 0;

            var inboundPallets = await (from d in _db.InboundDetails
                                        join i in _db.Inbounds on d.InboundId equals i.Id
                                        where d.SectionId == sectionId && i.ClientId == clientId && d.ProductId == productId
                                        select (int?)d.Pallets).SumAsync() ?? 0;

            var outboundCartons = await (from d in _db.OutboundDetails
                                         join o in _db.Outbounds on d.OutboundId equals o.Id
                                         where d.SectionId == sectionId && o.ClientId == clientId && d.ProductId == productId
                                         select (int?)d.Cartons).SumAsync() ?? 0;

            var outboundPallets = await (from d in _db.OutboundDetails
                                         join o in _db.Outbounds on d.OutboundId equals o.Id
                                         where d.SectionId == sectionId && o.ClientId == clientId && d.ProductId == productId
                                         select (int?)d.Pallets).SumAsync() ?? 0;

            var cartons = inboundCartons - outboundCartons;
            var pallets = inboundPallets - outboundPallets;

            var response = new StockResponse
            {
                ClientId = clientId,
                ProductId = productId,
                SectionId = sectionId,
                Cartons = cartons,
                Pallets = pallets
            };

            return Ok(response);
        }

        /// <summary>
        /// 4) Client totals across all sections. Returns one entry per section (sections with zero stock included).
        /// GET api/query/client/{clientId}/sections
        /// </summary>
        [HttpGet("client/{clientId}/sections")]
        public async Task<IActionResult> ClientTotalsAcrossSections([FromRoute] int clientId)
        {
            var clientExists = await _db.Clients.AnyAsync(c => c.Id == clientId);
            if (!clientExists) return NotFound(new { Message = $"Client {clientId} not found." });

            // Materialize inbound and outbound grouped sums with AsNoTracking to avoid EF tracked graphs in the response.
            var inboundList = await (from d in _db.InboundDetails
                                     join i in _db.Inbounds on d.InboundId equals i.Id
                                     where i.ClientId == clientId
                                     group d by d.SectionId into g
                                     select new { SectionId = g.Key, Cartons = g.Sum(x => x.Cartons), Pallets = g.Sum(x => x.Pallets) })
                                    .AsNoTracking()
                                    .ToListAsync();

            var outboundList = await (from d in _db.OutboundDetails
                                      join o in _db.Outbounds on d.OutboundId equals o.Id
                                      where o.ClientId == clientId
                                      group d by d.SectionId into g
                                      select new { SectionId = g.Key, Cartons = g.Sum(x => x.Cartons), Pallets = g.Sum(x => x.Pallets) })
                                     .AsNoTracking()
                                     .ToListAsync();

            var inboundDict = inboundList.ToDictionary(x => x.SectionId, x => (x.Cartons, x.Pallets));
            var outboundDict = outboundList.ToDictionary(x => x.SectionId, x => (x.Cartons, x.Pallets));

            var sections = await _db.Sections.AsNoTracking().OrderBy(s => s.Id).ToListAsync();

            var results = sections.Select(section =>
            {
                var inb = inboundDict.TryGetValue(section.Id, out var iv) ? iv : (0, 0);
                var outb = outboundDict.TryGetValue(section.Id, out var ov) ? ov : (0, 0);

                var cartons = inb.Item1 - outb.Item1;
                var pallets = inb.Item2 - outb.Item2;

                return new ClientSectionDto(clientId, section.Id, cartons, pallets);
            }).ToList();

            return Ok(results);
        }

        /// <summary>
        /// Stock balances. Returns totals per (Client, Product, Section) and supports filtering by optional query params.
        /// GET api/query/stock?clientId=&productId=&sectionId=
        /// </summary>
        [HttpGet("stock")]
        public async Task<IActionResult> GetStockBalances([FromQuery] int? clientId, [FromQuery] int? productId, [FromQuery] int? sectionId)
        {
            // Movements: inbound as positive, outbound as positive separately
            var inboundMovements = from d in _db.InboundDetails
                                   join i in _db.Inbounds on d.InboundId equals i.Id
                                   select new
                                   {
                                       ClientId = i.ClientId,
                                       ProductId = d.ProductId,
                                       SectionId = d.SectionId,
                                       InboundCartons = d.Cartons,
                                       InboundPallets = d.Pallets,
                                       OutboundCartons = 0,
                                       OutboundPallets = 0
                                   };

            var outboundMovements = from d in _db.OutboundDetails
                                    join o in _db.Outbounds on d.OutboundId equals o.Id
                                    select new
                                    {
                                        ClientId = o.ClientId,
                                        ProductId = d.ProductId,
                                        SectionId = d.SectionId,
                                        InboundCartons = 0,
                                        InboundPallets = 0,
                                        OutboundCartons = d.Cartons,
                                        OutboundPallets = d.Pallets
                                    };

            var allMovements = inboundMovements.Concat(outboundMovements);

            // apply filters if provided
            if (clientId.HasValue) allMovements = allMovements.Where(m => m.ClientId == clientId.Value);
            if (productId.HasValue) allMovements = allMovements.Where(m => m.ProductId == productId.Value);
            if (sectionId.HasValue) allMovements = allMovements.Where(m => m.SectionId == sectionId.Value);

            var grouped = from m in allMovements
                          group m by new { m.ClientId, m.ProductId, m.SectionId } into g
                          select new
                          {
                              g.Key.ClientId,
                              g.Key.ProductId,
                              g.Key.SectionId,
                              TotalInboundCartons = g.Sum(x => x.InboundCartons),
                              TotalInboundPallets = g.Sum(x => x.InboundPallets),
                              TotalOutboundCartons = g.Sum(x => x.OutboundCartons),
                              TotalOutboundPallets = g.Sum(x => x.OutboundPallets)
                          };

            var query = from g in grouped
                        join c in _db.Clients on g.ClientId equals c.Id
                        join p in _db.Products on g.ProductId equals p.Id
                        join s in _db.Sections on g.SectionId equals s.Id
                        select new
                        {
                            ClientId = g.ClientId,
                            ClientName = c.Name,
                            ProductId = g.ProductId,
                            ProductName = p.Name,
                            SectionId = g.SectionId,
                            SectionName = s.Name,
                            TotalInboundCartons = g.TotalInboundCartons,
                            TotalInboundPallets = g.TotalInboundPallets,
                            TotalOutboundCartons = g.TotalOutboundCartons,
                            TotalOutboundPallets = g.TotalOutboundPallets,
                            CurrentCartons = g.TotalInboundCartons - g.TotalOutboundCartons,
                            CurrentPallets = g.TotalInboundPallets - g.TotalOutboundPallets
                        };

            var results = await query.ToListAsync();

            return Ok(results.Select(r => new
            {
                r.ClientId,
                r.ClientName,
                r.ProductId,
                r.ProductName,
                r.SectionId,
                r.SectionName,
                TotalInboundCartons = r.TotalInboundCartons,
                TotalOutboundCartons = r.TotalOutboundCartons,
                CurrentCartons = r.CurrentCartons,
                TotalInboundPallets = r.TotalInboundPallets,
                TotalOutboundPallets = r.TotalOutboundPallets,
                CurrentPallets = r.CurrentPallets
            }));
        }
    }
}
