using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using StudyGroup.Data.Interfaces;
using StudyGroup.Data.Models;
using StudyGroup.Data.SqlQueries;

namespace StudyGroup.Data.Repositories
{
    // Implements IEntityTypeRepository using Dapper for data access
    public class EntityTypeRepository : IEntityTypeRepository
    {
        private readonly IDbConnection _dbConnection;

        // Injects the database connection
        public EntityTypeRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        // Gets all entity types from the database
        public async Task<IEnumerable<EntityType>> GetAllAsync()
        {
            return await _dbConnection.QueryAsync<EntityType>(EntityTypeSql.GetAll);
        }

        // Gets a single entity type by its ID
        public async Task<EntityType?> GetByIdAsync(int entityTypeId)
        {
            return await _dbConnection.QueryFirstOrDefaultAsync<EntityType>(EntityTypeSql.GetById, new { EntityTypeId = entityTypeId });
        }

        // Inserts a new entity type and returns its ID
        public async Task<int> CreateAsync(EntityType entityType)
        {
            return await _dbConnection.QuerySingleAsync<int>(EntityTypeSql.Insert, entityType);
        }

        // Updates an existing entity type
        public async Task<bool> UpdateAsync(EntityType entityType)
        {
            var affected = await _dbConnection.ExecuteAsync(EntityTypeSql.Update, entityType);
            return affected > 0;
        }

        // Deletes an entity type by its ID
        public async Task<bool> DeleteAsync(int entityTypeId)
        {
            var affected = await _dbConnection.ExecuteAsync(EntityTypeSql.Delete, new { EntityTypeId = entityTypeId });
            return affected > 0;
        }
    }
}
