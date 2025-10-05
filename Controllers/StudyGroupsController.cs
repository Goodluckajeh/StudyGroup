using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudyGroup.Repository.Data;
using StudyGroup.Service.Interface;
using System.Threading.Tasks;
using StudyGroup.Repository.Models;

namespace StudyGroup.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudyGroupsController : ControllerBase
    {
        private readonly IStudyGroupService _studyGroupService;
        private readonly IGroupMemberService _groupMemberService;
        private readonly AppDbContext _context; // Needed for course lookup

        public StudyGroupsController(
            IStudyGroupService studyGroupService,
            IGroupMemberService groupMemberService,
            AppDbContext context)
        {
            _studyGroupService = studyGroupService;
            _groupMemberService = groupMemberService;
            _context = context;
        }

        // GET: api/studygroups
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var groups = await _studyGroupService.GetAllAsync();
            return Ok(groups);
        }

        // GET: api/studygroups/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var group = await _studyGroupService.GetAsync(id);
            if (group == null) return NotFound();
            return Ok(group);
        }

        // POST: api/studygroups
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateStudyGroupDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.CourseName) || dto.CreatorId <= 0)
                return BadRequest("CourseName and CreatorId are required.");

            // Lookup the course by CourseName (case-insensitive, trimmed)
            var course = await _context.Courses
                .FirstOrDefaultAsync(c =>
                    c.CourseName != null &&
                    c.CourseName.Trim().ToLower() == dto.CourseName.Trim().ToLower()
                );

            if (course == null)
                return BadRequest($"Course with name '{dto.CourseName}' not found.");

            // Map DTO to entity
            var group = new StudyGroups
            {
                CourseId = course.CourseId,
                CreatorId = dto.CreatorId,
                Topic = dto.Topic,
                TimeSlot = dto.TimeSlot,
                Description = dto.Description
            };

            // Create the study group
            var createdGroup = await _studyGroupService.CreateAsync(group);

            // Automatically add creator as approved member
            await _groupMemberService.AddCreatorAsMemberAsync(createdGroup.GroupId, createdGroup.CreatorId);

            return CreatedAtAction(nameof(Get), new { id = createdGroup.GroupId }, createdGroup);
        }

        // PUT: api/studygroups/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] StudyGroups updated)
        {
            var group = await _studyGroupService.GetAsync(id);
            if (group == null) return NotFound();

            group.Topic = updated.Topic;
            group.TimeSlot = updated.TimeSlot;
            group.Description = updated.Description;

            await _studyGroupService.UpdateAsync(group, updated);
            return NoContent();
        }

        // DELETE: api/studygroups/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var group = await _studyGroupService.GetAsync(id);
            if (group == null) return NotFound();

            await _studyGroupService.DeleteAsync(id);
            return NoContent();
        }
    }
}
