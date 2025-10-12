using System.Collections.Generic;
using System.Threading.Tasks;
using StudyGroup.Data.Models;

namespace StudyGroup.Data.Interfaces
{
    // Interface for DayOfWeek repository, defines CRUD operations
    public interface IDayOfWeekRepository
    {
        Task<IEnumerable<DayOfWeekEntity>> GetAllAsync();
        Task<DayOfWeekEntity?> GetByIdAsync(int dayId);
        Task<int> CreateAsync(DayOfWeekEntity day);
        Task<bool> UpdateAsync(DayOfWeekEntity day);
        Task<bool> DeleteAsync(int dayId);
    }
}
