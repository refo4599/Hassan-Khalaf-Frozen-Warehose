using Frozen_Warehouse.Application.DTOs.Section;

namespace Frozen_Warehouse.Application.Interfaces.IServices
{
    public interface ISectionService
    {
        Task<IEnumerable<SectionDto>> GetAllAsync();
    }
}
