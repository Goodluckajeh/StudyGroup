using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudyGroup.Data;
using StudyGroup.Models;

namespace StudyGroup.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AvailabilitySlotsController : ControllerBase
    {
        private readonly AppDbContext _context;
        public AvailabilitySlotsController(AppDbContext context) => _context = context;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _context.AvailabilitySlots.ToListAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var slot = await _context.AvailabilitySlots.FindAsync(id);
            if (slot == null) return NotFound();
            return Ok(slot);
        }

        [HttpPost]
        public async Task<IActionResult> Create(AvailabilitySlot slot)
        {
            _context.AvailabilitySlots.Add(slot);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = slot.AvailabilityId }, slot);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, AvailabilitySlot updated)
        {
            var slot = await _context.AvailabilitySlots.FindAsync(id);
            if (slot == null) return NotFound();

            slot.DayOfWeek = updated.DayOfWeek;
            slot.StartTime = updated.StartTime;
            slot.EndTime = updated.EndTime;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var slot = await _context.AvailabilitySlots.FindAsync(id);
            if (slot == null) return NotFound();

            _context.AvailabilitySlots.Remove(slot);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
