using System.Collections.Generic;
using System.Threading.Tasks;
using StudyGroup.Data.Models;
using StudyGroup.Data.Interfaces;
using StudyGroup.Service.Interfaces;
using StudyGroup.Service.DTOs;
using System.Linq;

namespace StudyGroup.Service.Services
{
    // Implements IDayOfWeekService, contains business logic for days of week
    public class DayOfWeekService : IDayOfWeekService
    {
        private readonly IDayOfWeekRepository _repository;

        // Injects the day of week repository
        public DayOfWeekService(IDayOfWeekRepository repository)
        {
            _repository = repository;
        }

        // Maps DayOfWeekEntity model (PascalCase) to DayOfWeekDto (PascalCase)
        private DayOfWeekDto MapToDto(DayOfWeekEntity day) => new DayOfWeekDto
        {
            DayId = day.DayId,
            DayName = day.DayName
        };

        // Gets all days of week
        public async Task<IEnumerable<DayOfWeekDto>> GetAllDaysAsync()
        {
            var days = await _repository.GetAllAsync();
            return days.Select(MapToDto);
        }

        // Gets a day of week by ID
        public async Task<DayOfWeekDto?> GetDayByIdAsync(int dayId)
        {
            var day = await _repository.GetByIdAsync(dayId);
            return day == null ? null : MapToDto(day);
        }

        // Creates a new day of week
        public async Task<int> CreateDayAsync(CreateDayOfWeekDto dayDto)
        {
            var day = new DayOfWeekEntity
            {
                DayName = dayDto.DayName
            };
            return await _repository.CreateAsync(day);
        }

        // Updates an existing day of week
        public async Task<bool> UpdateDayAsync(int dayId, UpdateDayOfWeekDto dayDto)
        {
            var day = await _repository.GetByIdAsync(dayId);
            if (day == null) return false;
            day.DayName = dayDto.DayName;
            return await _repository.UpdateAsync(day);
        }

        // Deletes a day of week by ID
        public async Task<bool> DeleteDayAsync(int dayId)
        {
            return await _repository.DeleteAsync(dayId);
        }
    }
}
