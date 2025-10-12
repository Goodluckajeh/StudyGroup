namespace StudyGroup.Service.DTOs
{
    // DTO for returning study group data
    public class StudyGroupDto
    {
        public int GroupId { get; set; }
        public int CourseId { get; set; }           // Keep for internal use
        public string CourseName { get; set; } = string.Empty;  // Add for display
        public int CreatorId { get; set; }
        public string? Topic { get; set; }
        public string? TimeSlot { get; set; }
        public string? Description { get; set; }
    }

    // DTO for creating a study group (user-friendly)
    public class CreateStudyGroupDto
    {
        public string CourseName { get; set; } = string.Empty;  // Use course name instead of ID
        public int CreatorId { get; set; }
        public string? Topic { get; set; }
        public string? TimeSlot { get; set; }
        public string? Description { get; set; }
    }

    // DTO for updating a study group
    public class UpdateStudyGroupDto
    {
        public string CourseName { get; set; } = string.Empty;  // Use course name instead of ID
        public int CreatorId { get; set; }
        public string? Topic { get; set; }
        public string? TimeSlot { get; set; }
        public string? Description { get; set; }
    }
}
