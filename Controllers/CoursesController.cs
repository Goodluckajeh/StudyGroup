using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using StudyGroup.Repository.Models;
using StudyGroup.Service.Interface;
using StudyGroup.Service.Models.Services;
using System.Threading.Tasks;

namespace StudyGroup.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _service;

        public CoursesController(ICourseService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var course = await _service.GetAsync(id);
            if (course == null) return NotFound();
            return Ok(course);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Course course)
        {
            var created = await _service.CreateAsync(course);
            return CreatedAtAction(nameof(Get), new { id = created.CourseId }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Course updated)
        {
            var course = await _service.GetAsync(id);
            if (course == null) return NotFound();

            await _service.UpdateAsync(course, updated);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
