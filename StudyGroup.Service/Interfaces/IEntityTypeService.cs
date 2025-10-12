using System.Collections.Generic;
using System.Threading.Tasks;
using StudyGroup.Service.DTOs;

namespace StudyGroup.Service.Interfaces
{
    // Interface for EntityType service, defines business logic methods
    public interface IEntityTypeService
    {
        Task<IEnumerable<EntityTypeDto>> GetAllEntityTypesAsync();
        Task<EntityTypeDto?> GetEntityTypeByIdAsync(int entityTypeId);
        Task<int> CreateEntityTypeAsync(CreateEntityTypeDto entityTypeDto);
        Task<bool> UpdateEntityTypeAsync(int entityTypeId, UpdateEntityTypeDto entityTypeDto);
        Task<bool> DeleteEntityTypeAsync(int entityTypeId);
    }
}
