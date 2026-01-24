namespace Frozen_Warehouse.Application.DTOs.Stock
{
    public class StockResponse
    {
        public Guid ClientId { get; set; }
        public Guid ProductId { get; set; }
        public Guid SectionId { get; set; }
        public decimal Quantity { get; set; }
    }
}