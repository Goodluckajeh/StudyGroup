using System.Collections.Generic;
using System.Threading.Tasks;
using StudyGroup.Data.Models;

namespace StudyGroup.Data.Interfaces
{
    // Interface for MembershipStatus repository, defines CRUD operations
    public interface IMembershipStatusRepository
    {
        Task<IEnumerable<MembershipStatus>> GetAllAsync();
        Task<MembershipStatus?> GetByIdAsync(int statusId);
        Task<int> CreateAsync(MembershipStatus status);
        Task<bool> UpdateAsync(MembershipStatus status);
        Task<bool> DeleteAsync(int statusId);
    }
}
