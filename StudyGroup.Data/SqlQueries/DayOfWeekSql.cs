namespace StudyGroup.Data.SqlQueries
{
    // Contains SQL queries for DayOfWeek entity CRUD operations
    public static class DayOfWeekSql
    {
        public const string GetAll = @"SELECT * FROM DaysOfWeek";
        public const string GetById = @"SELECT * FROM DaysOfWeek WHERE DayId = @DayId";
        public const string Insert = @"INSERT INTO DaysOfWeek (DayName) VALUES (@DayName); SELECT CAST(SCOPE_IDENTITY() as int);";
        public const string Update = @"UPDATE DaysOfWeek SET DayName = @DayName WHERE DayId = @DayId";
        public const string Delete = @"DELETE FROM DaysOfWeek WHERE DayId = @DayId";
    }
}
