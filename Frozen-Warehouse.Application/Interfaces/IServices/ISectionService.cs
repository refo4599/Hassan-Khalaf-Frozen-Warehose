using Frozen_Warehouse.Application.DTOs.Section;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Frozen_Warehouse.Application.Interfaces.IServices
{
    public interface ISectionService
    {
        Task<IEnumerable<SectionDTO>> GetAllAsync();
    }

}
