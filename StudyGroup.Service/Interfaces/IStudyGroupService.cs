using System.Collections.Generic;
using System.Threading.Tasks;
using StudyGroup.Service.DTOs;

namespace StudyGroup.Service.Interfaces
{
    public interface IStudyGroupService
    {
        Task<IEnumerable<StudyGroupDto>> GetAllGroupsAsync();
        Task<StudyGroupDto?> GetGroupByIdAsync(int groupId);
        Task<int> CreateGroupAsync(CreateStudyGroupDto groupDto);
        Task<bool> UpdateGroupAsync(int groupId, UpdateStudyGroupDto groupDto);
        Task<bool> DeleteGroupAsync(int groupId);
        Task<int?> GetGroupCreatorIdAsync(int groupId);
    }
}
