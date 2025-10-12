using System.Collections.Generic;
using System.Threading.Tasks;
using StudyGroup.Data.Models;

namespace StudyGroup.Data.Interfaces
{
    // Interface for Tag repository, defines CRUD operations
    public interface ITagRepository
    {
        Task<IEnumerable<Tag>> GetAllAsync();
        Task<Tag?> GetByIdAsync(int tagId);
        Task<int> CreateAsync(Tag tag);
        Task<bool> UpdateAsync(Tag tag);
        Task<bool> DeleteAsync(int tagId);
    }
}
