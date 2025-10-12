namespace StudyGroup.Service.DTOs
{
    // DTO for returning course data
    public class CourseDto
    {
        public int CourseId { get; set; }
        public string CourseCode { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    // DTO for creating a course
    public class CreateCourseDto
    {
        public string CourseCode { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    // DTO for updating a course
    public class UpdateCourseDto
    {
        public string CourseCode { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
