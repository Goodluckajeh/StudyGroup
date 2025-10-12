using System.Collections.Generic;
using System.Threading.Tasks;
using StudyGroup.Service.DTOs;

namespace StudyGroup.Service.Interfaces
{
    // Interface for Tag service, defines business logic methods
    public interface ITagService
    {
        Task<IEnumerable<TagDto>> GetAllTagsAsync();
        Task<TagDto?> GetTagByIdAsync(int tagId);
        Task<int> CreateTagAsync(CreateTagDto tagDto);
        Task<bool> UpdateTagAsync(int tagId, UpdateTagDto tagDto);
        Task<bool> DeleteTagAsync(int tagId);
        
        // New methods for entity-specific operations
        Task<IEnumerable<TagDto>> GetTagsByEntityAsync(int entityTypeId, int entityId);
        Task<IEnumerable<TagDto>> GetTagsByEntityTypeAsync(int entityTypeId);
    }
}
