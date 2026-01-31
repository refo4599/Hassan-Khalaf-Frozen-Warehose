using Frozen_Warehouse.Application.DTOs.Section;
using Frozen_Warehouse.Application.Interfaces.IServices;
using Frozen_Warehouse.Domain.Interfaces;

namespace Frozen_Warehouse.Application.Services
{
    public class SectionService : ISectionService
    {
        private readonly ISectionRepository _sectionRepo;

        public SectionService(ISectionRepository sectionRepo)
        {
            _sectionRepo = sectionRepo;
        }

        public async Task<IEnumerable<SectionDTO>> GetAllAsync()
        {
            var sections = await _sectionRepo.GetAllAsync();
            return sections.Select(s => new SectionDTO { Id = s.Id, Name = s.Name });
        }
    }
}
