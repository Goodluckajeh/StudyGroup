using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using StudyGroup.Data.Interfaces;
using StudyGroup.Data.Models;
using StudyGroup.Data.SqlQueries;

namespace StudyGroup.Data.Repositories
{
    // Implements ITagRepository using Dapper for data access
    public class TagRepository : ITagRepository
    {
        private readonly IDbConnection _dbConnection;

        // Injects the database connection
        public TagRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        // Gets all tags from the database
        public async Task<IEnumerable<Tag>> GetAllAsync()
        {
            return await _dbConnection.QueryAsync<Tag>(TagSql.GetAll);
        }

        // Gets a single tag by its ID
        public async Task<Tag?> GetByIdAsync(int tagId)
        {
            return await _dbConnection.QueryFirstOrDefaultAsync<Tag>(TagSql.GetById, new { TagId = tagId });
        }

        // Inserts a new tag and returns its ID
        public async Task<int> CreateAsync(Tag tag)
        {
            return await _dbConnection.QuerySingleAsync<int>(TagSql.Insert, tag);
        }

        // Updates an existing tag
        public async Task<bool> UpdateAsync(Tag tag)
        {
            var affected = await _dbConnection.ExecuteAsync(TagSql.Update, tag);
            return affected > 0;
        }

        // Deletes a tag by its ID
        public async Task<bool> DeleteAsync(int tagId)
        {
            var affected = await _dbConnection.ExecuteAsync(TagSql.Delete, new { TagId = tagId });
            return affected > 0;
        }
    }
}
