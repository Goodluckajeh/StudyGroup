namespace StudyGroup.Data.SqlQueries
{
    // Contains SQL queries for Course entity CRUD operations
    public static class CourseSql
    {
        public const string GetAll = @"SELECT * FROM Courses";
        public const string GetById = @"SELECT * FROM Courses WHERE CourseId = @CourseId";
        public const string GetByName = @"SELECT * FROM Courses WHERE CourseName = @CourseName";
        public const string GetByCode = @"SELECT * FROM Courses WHERE CourseCode = @CourseCode";
        public const string CheckExistsByCode = @"SELECT CASE WHEN EXISTS(SELECT 1 FROM Courses WHERE CourseCode = @CourseCode) THEN 1 ELSE 0 END";
        public const string Insert = @"INSERT INTO Courses (CourseCode, CourseName, Description) VALUES (@CourseCode, @CourseName, @Description); SELECT CAST(SCOPE_IDENTITY() as int);";
        public const string Update = @"UPDATE Courses SET CourseCode = @CourseCode, CourseName = @CourseName, Description = @Description WHERE CourseId = @CourseId";
        public const string Delete = @"DELETE FROM Courses WHERE CourseId = @CourseId";
    }
}
