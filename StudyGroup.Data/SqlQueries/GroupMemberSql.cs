namespace StudyGroup.Data.SqlQueries
{
    // Contains SQL queries for GroupMember entity CRUD operations
    public static class GroupMemberSql
    {
        // Basic CRUD queries
        public const string GetAll = @"SELECT * FROM GroupMembers";
        public const string GetById = @"SELECT * FROM GroupMembers WHERE GroupMemberId = @GroupMemberId";
        public const string Insert = @"INSERT INTO GroupMembers (GroupId, UserId, StatusId) VALUES (@GroupId, @UserId, @StatusId); SELECT CAST(SCOPE_IDENTITY() as int);";
        public const string Update = @"UPDATE GroupMembers SET GroupId = @GroupId, UserId = @UserId, StatusId = @StatusId WHERE GroupMemberId = @GroupMemberId";
        public const string Delete = @"DELETE FROM GroupMembers WHERE GroupMemberId = @GroupMemberId";

        // Specialized queries for membership management
        public const string GetByGroupId = @"SELECT * FROM GroupMembers WHERE GroupId = @GroupId";
        public const string GetByUserId = @"SELECT * FROM GroupMembers WHERE UserId = @UserId";
        public const string GetByGroupIdAndStatus = @"SELECT * FROM GroupMembers WHERE GroupId = @GroupId AND StatusId = @StatusId";
        public const string GetByGroupIdAndUserId = @"SELECT * FROM GroupMembers WHERE GroupId = @GroupId AND UserId = @UserId";
    }
}
