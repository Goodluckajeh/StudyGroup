namespace StudyGroup.Service.DTOs
{
    // DTO for returning day of week data
    public class DayOfWeekDto
    {
        public int DayId { get; set; }
        public string DayName { get; set; } = string.Empty;
    }

    // DTO for creating a day of week
    public class CreateDayOfWeekDto
    {
        public string DayName { get; set; } = string.Empty;
    }

    // DTO for updating a day of week
    public class UpdateDayOfWeekDto
    {
        public string DayName { get; set; } = string.Empty;
    }
}
