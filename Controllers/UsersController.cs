using Microsoft.AspNetCore.Mvc;
using StudyGroup.Repository.Models;
using StudyGroup.Service.Interface;
using System;
using System.Threading.Tasks;

namespace StudyGroup.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _service;

        public UsersController(IUserService service)
        {
            _service = service;
        }

        // GET: api/users
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _service.GetAllAsync();
            return Ok(users);
        }

        // GET: api/users/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var user = await _service.GetAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        // POST: api/users/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
        {
            try
            {
                var user = await _service.RegisterUserAsync(dto);
                return CreatedAtAction(nameof(Get), new { id = user.UserId }, user);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/users/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, User updated)
        {
            var user = await _service.GetAsync(id);
            if (user == null) return NotFound();

            await _service.UpdateAsync(user, updated);
            return NoContent();
        }

        // DELETE: api/users/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _service.GetAsync(id);
            if (user == null) return NotFound();

            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
