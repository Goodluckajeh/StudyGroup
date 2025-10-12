using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using StudyGroup.Data.Interfaces;
using StudyGroup.Data.Models;
using StudyGroup.Data.SqlQueries;

namespace StudyGroup.Data.Repositories
{
    // Implements IMembershipStatusRepository using Dapper for data access
    public class MembershipStatusRepository : IMembershipStatusRepository
    {
        private readonly IDbConnection _dbConnection;

        // Injects the database connection
        public MembershipStatusRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        // Gets all membership statuses from the database
        public async Task<IEnumerable<MembershipStatus>> GetAllAsync()
        {
            return await _dbConnection.QueryAsync<MembershipStatus>(MembershipStatusSql.GetAll);
        }

        // Gets a single membership status by its ID
        public async Task<MembershipStatus?> GetByIdAsync(int statusId)
        {
            return await _dbConnection.QueryFirstOrDefaultAsync<MembershipStatus>(MembershipStatusSql.GetById, new { StatusId = statusId });
        }

        // Inserts a new membership status and returns its ID
        public async Task<int> CreateAsync(MembershipStatus status)
        {
            return await _dbConnection.QuerySingleAsync<int>(MembershipStatusSql.Insert, status);
        }

        // Updates an existing membership status
        public async Task<bool> UpdateAsync(MembershipStatus status)
        {
            var affected = await _dbConnection.ExecuteAsync(MembershipStatusSql.Update, status);
            return affected > 0;
        }

        // Deletes a membership status by its ID
        public async Task<bool> DeleteAsync(int statusId)
        {
            var affected = await _dbConnection.ExecuteAsync(MembershipStatusSql.Delete, new { StatusId = statusId });
            return affected > 0;
        }
    }
}
