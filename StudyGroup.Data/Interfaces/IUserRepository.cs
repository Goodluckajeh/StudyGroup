using System.Collections.Generic;
using System.Threading.Tasks;
using StudyGroup.Data.Models;

namespace StudyGroup.Data.Interfaces
{
    /// <summary>
    /// Interface for User repository, defines CRUD operations.
    /// </summary>
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int userId);
        Task<User?> GetByEmailAsync(string email);
        Task<int> CreateAsync(User user);
        Task<bool> UpdateAsync(User user);
        Task<bool> DeleteAsync(int userId);
    }
}
