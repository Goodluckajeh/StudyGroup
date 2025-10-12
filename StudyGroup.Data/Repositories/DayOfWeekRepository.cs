using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using StudyGroup.Data.Interfaces;
using StudyGroup.Data.Models;
using StudyGroup.Data.SqlQueries;

namespace StudyGroup.Data.Repositories
{
    // Implements IDayOfWeekRepository using Dapper for data access
    public class DayOfWeekRepository : IDayOfWeekRepository
    {
        private readonly IDbConnection _dbConnection;

        // Injects the database connection
        public DayOfWeekRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        // Gets all days of week from the database
        public async Task<IEnumerable<DayOfWeekEntity>> GetAllAsync()
        {
            return await _dbConnection.QueryAsync<DayOfWeekEntity>(DayOfWeekSql.GetAll);
        }

        // Gets a single day of week by its ID
        public async Task<DayOfWeekEntity?> GetByIdAsync(int dayId)
        {
            return await _dbConnection.QueryFirstOrDefaultAsync<DayOfWeekEntity>(DayOfWeekSql.GetById, new { DayId = dayId });
        }

        // Inserts a new day of week and returns its ID
        public async Task<int> CreateAsync(DayOfWeekEntity day)
        {
            return await _dbConnection.QuerySingleAsync<int>(DayOfWeekSql.Insert, day);
        }

        // Updates an existing day of week
        public async Task<bool> UpdateAsync(DayOfWeekEntity day)
        {
            var affected = await _dbConnection.ExecuteAsync(DayOfWeekSql.Update, day);
            return affected > 0;
        }

        // Deletes a day of week by its ID
        public async Task<bool> DeleteAsync(int dayId)
        {
            var affected = await _dbConnection.ExecuteAsync(DayOfWeekSql.Delete, new { DayId = dayId });
            return affected > 0;
        }
    }
}
