using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudyGroup.Repository.Data;

[Route("api/[controller]")]
[ApiController]
public class EntityTypesController : ControllerBase
{
    private readonly AppDbContext _context;
    public EntityTypesController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _context.EntityTypes.ToListAsync());
}
