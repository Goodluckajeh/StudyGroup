using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudyGroup.Repository.Data;

namespace StudyGroup.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DaysOfWeekController : ControllerBase
    {
        private readonly AppDbContext _context;
        public DaysOfWeekController(AppDbContext context) => _context = context;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _context.DaysOfWeek.ToListAsync());
    }
}