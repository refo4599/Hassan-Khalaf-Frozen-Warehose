namespace Frozen_Warehouse.Domain.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Section:BaseEntity
    {
        public string Name { get; set; } = null!;


    }
}
