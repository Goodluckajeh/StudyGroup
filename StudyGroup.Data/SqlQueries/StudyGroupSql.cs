namespace StudyGroup.Data.SqlQueries
{
    public static class StudyGroupSql
    {
        public const string GetAll = @"SELECT * FROM StudyGroups";
        public const string GetById = @"SELECT * FROM StudyGroups WHERE GroupId = @GroupId";
        public const string Insert = @"INSERT INTO StudyGroups (CourseId, CreatorId, Topic, TimeSlot, Description) VALUES (@CourseId, @CreatorId, @Topic, @TimeSlot, @Description); SELECT CAST(SCOPE_IDENTITY() as int);";
        public const string Update = @"UPDATE StudyGroups SET CourseId = @CourseId, CreatorId = @CreatorId, Topic = @Topic, TimeSlot = @TimeSlot, Description = @Description WHERE GroupId = @GroupId";
        public const string Delete = @"DELETE FROM StudyGroups WHERE GroupId = @GroupId";
    }
}
