using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudyGroup.Data;
using StudyGroup.Models;

namespace StudyGroup.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudyGroupsController : ControllerBase
    {
        private readonly AppDbContext _context;
        public StudyGroupsController(AppDbContext context) => _context = context;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _context.StudyGroups.ToListAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var group = await _context.StudyGroups.FindAsync(id);
            if (group == null) return NotFound();
            return Ok(group);
        }

        [HttpPost]
        public async Task<IActionResult> Create(StudyGroups group)
        {
            _context.StudyGroups.Add(group);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = group.GroupId }, group);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, StudyGroups updated)
        {
            var group = await _context.StudyGroups.FindAsync(id);
            if (group == null) return NotFound();

            group.Topic = updated.Topic;
            group.TimeSlot = updated.TimeSlot;
            group.Description = updated.Description;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var group = await _context.StudyGroups.FindAsync(id);
            if (group == null) return NotFound();

            _context.StudyGroups.Remove(group);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
