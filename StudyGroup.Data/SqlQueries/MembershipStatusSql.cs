namespace StudyGroup.Data.SqlQueries
{
    // Contains SQL queries for MembershipStatus entity CRUD operations
    public static class MembershipStatusSql
    {
        public const string GetAll = @"SELECT * FROM MembershipStatuses";
        public const string GetById = @"SELECT * FROM MembershipStatuses WHERE StatusId = @StatusId";
        public const string Insert = @"INSERT INTO MembershipStatuses (StatusName) VALUES (@StatusName); SELECT CAST(SCOPE_IDENTITY() as int);";
        public const string Update = @"UPDATE MembershipStatuses SET StatusName = @StatusName WHERE StatusId = @StatusId";
        public const string Delete = @"DELETE FROM MembershipStatuses WHERE StatusId = @StatusId";
    }
}
