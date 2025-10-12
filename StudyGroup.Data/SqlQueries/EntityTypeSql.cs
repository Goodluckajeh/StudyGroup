namespace StudyGroup.Data.SqlQueries
{
    // Contains SQL queries for EntityType entity CRUD operations
    public static class EntityTypeSql
    {
        public const string GetAll = @"SELECT * FROM EntityTypes";
        public const string GetById = @"SELECT * FROM EntityTypes WHERE EntityTypeId = @EntityTypeId";
        public const string Insert = @"INSERT INTO EntityTypes (EntityTypeName) VALUES (@EntityTypeName); SELECT CAST(SCOPE_IDENTITY() as int);";
        public const string Update = @"UPDATE EntityTypes SET EntityTypeName = @EntityTypeName WHERE EntityTypeId = @EntityTypeId";
        public const string Delete = @"DELETE FROM EntityTypes WHERE EntityTypeId = @EntityTypeId";
    }
}
