namespace StudyGroup.Data.Models
{
    public class GroupMember
    {
        public int GroupMemberId { get; set; }
        public int GroupId { get; set; }
        public int UserId { get; set; }
        public int StatusId { get; set; }
    }
}
