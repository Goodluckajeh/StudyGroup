using System.Collections.Generic;
using System.Threading.Tasks;
using StudyGroup.Data.Models;

namespace StudyGroup.Data.Interfaces
{
    // Interface for EntityType repository, defines CRUD operations
    public interface IEntityTypeRepository
    {
        Task<IEnumerable<EntityType>> GetAllAsync();
        Task<EntityType?> GetByIdAsync(int entityTypeId);
        Task<int> CreateAsync(EntityType entityType);
        Task<bool> UpdateAsync(EntityType entityType);
        Task<bool> DeleteAsync(int entityTypeId);
    }
}
