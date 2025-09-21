using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudyGroup.Data;
using StudyGroup.Models;

namespace StudyGroup.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupMembersController : ControllerBase
    {
        private readonly AppDbContext _context;
        public GroupMembersController(AppDbContext context) => _context = context;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _context.GroupMembers.ToListAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var member = await _context.GroupMembers.FindAsync(id);
            if (member == null) return NotFound();
            return Ok(member);
        }

        [HttpPost]
        public async Task<IActionResult> Create(GroupMember member)
        {
            _context.GroupMembers.Add(member);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = member.GroupMemberId }, member);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, GroupMember updated)
        {
            var member = await _context.GroupMembers.FindAsync(id);
            if (member == null) return NotFound();

            member.Status = updated.Status;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var member = await _context.GroupMembers.FindAsync(id);
            if (member == null) return NotFound();

            _context.GroupMembers.Remove(member);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
