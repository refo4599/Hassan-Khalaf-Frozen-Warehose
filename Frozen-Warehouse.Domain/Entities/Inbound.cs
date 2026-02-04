namespace Frozen_Warehouse.Domain.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Inbound:BaseEntity
    {
        public int ClientId { get; set; }
        public Client Client { get; set; } = null!;


        // Navigation
        public IList<InboundDetail> Details { get; set; } = new List<InboundDetail>();
    }
}
