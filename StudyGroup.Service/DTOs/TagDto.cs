namespace StudyGroup.Service.DTOs
{
    // DTO for returning tag data
    public class TagDto
    {
        public int TagId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int EntityTypeId { get; set; }
        public int EntityId { get; set; }
    }

    // DTO for creating a tag
    public class CreateTagDto
    {
        public string Name { get; set; } = string.Empty;
        public int EntityTypeId { get; set; }
        public int EntityId { get; set; }
    }

    // DTO for updating a tag
    public class UpdateTagDto
    {
        public string Name { get; set; } = string.Empty;
        public int EntityTypeId { get; set; }
        public int EntityId { get; set; }
    }
}
