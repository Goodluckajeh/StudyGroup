namespace StudyGroup.Data.Models
{
    public class Tag
    {
        public int TagId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int EntityTypeId { get; set; }
        public int EntityId { get; set; }
    }
}
