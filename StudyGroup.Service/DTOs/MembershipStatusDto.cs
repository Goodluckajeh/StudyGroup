namespace StudyGroup.Service.DTOs
{
    // DTO for returning membership status data
    public class MembershipStatusDto
    {
        public int StatusId { get; set; }
        public string StatusName { get; set; } = string.Empty;
    }

    // DTO for creating a membership status
    public class CreateMembershipStatusDto
    {
        public string StatusName { get; set; } = string.Empty;
    }

    // DTO for updating a membership status
    public class UpdateMembershipStatusDto
    {
        public string StatusName { get; set; } = string.Empty;
    }
}
