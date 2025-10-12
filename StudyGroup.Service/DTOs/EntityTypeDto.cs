namespace StudyGroup.Service.DTOs
{
    // DTO for returning entity type data
    public class EntityTypeDto
    {
        public int EntityTypeId { get; set; }
        public string EntityTypeName { get; set; } = string.Empty;
    }

    // DTO for creating an entity type
    public class CreateEntityTypeDto
    {
        public string EntityTypeName { get; set; } = string.Empty;
    }

    // DTO for updating an entity type
    public class UpdateEntityTypeDto
    {
        public string EntityTypeName { get; set; } = string.Empty;
    }
}
