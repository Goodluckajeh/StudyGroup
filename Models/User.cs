namespace StudyGroup.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Skills { get; set; }
        public bool Visibility { get; set; }
        public string Bio { get; set; }
    }

    public class Course
    {
        public int CourseId { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string Description { get; set; }
    }

    public class StudyGroups
    {
        public int GroupId { get; set; }
        public int CourseId { get; set; }
        public int CreatorId { get; set; }
        public string Topic { get; set; }
        public string TimeSlot { get; set; }
        public string Description { get; set; }
    }

    public class GroupMember
    {
        public int GroupMemberId { get; set; }
        public int GroupId { get; set; }
        public int UserId { get; set; }
        public string Status { get; set; }
    }

    public class AvailabilitySlot
    {
        public int AvailabilityId { get; set; }
        public int UserId { get; set; }
        public string DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }

    public class Tag
    {
        public int TagId { get; set; }
        public string Name { get; set; }
        public string EntityType { get; set; }
        public int EntityId { get; set; }
    }
}
