using Microsoft.AspNetCore.Mvc;
using StudyGroup.Service.Interfaces;

namespace StudyGroup.Api.Controllers
{
    // Read-only API controller for EntityType endpoints (lookup/enum table)
    [ApiController]
    [Route("api/[controller]")]
    public class EntityTypesController : ControllerBase
    {
        private readonly IEntityTypeService _service;

        // Injects the entity type service
        public EntityTypesController(IEntityTypeService service)
        {
            _service = service;
        }

        // GET: api/entitytypes
        // Returns all entity types (read-only lookup data)
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var types = await _service.GetAllEntityTypesAsync();
            return Ok(types);
        }

        // GET: api/entitytypes/{id}
        // Returns a specific entity type by ID (read-only lookup data)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var type = await _service.GetEntityTypeByIdAsync(id);
            if (type == null) return NotFound();
            return Ok(type);
        }

        // POST, PUT, DELETE endpoints removed - this is lookup/enum data
        // If modifications are needed, they should be done through database seeding or admin tools
    }
}
