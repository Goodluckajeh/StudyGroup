using Microsoft.AspNetCore.Mvc;
using StudyGroup.Repository.Models;
using StudyGroup.Service.Interface;

namespace StudyGroup.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AvailabilitySlotsController : ControllerBase
    {
        private readonly IAvailabilityService _service;
        private readonly ILookupService _lookup;

        public AvailabilitySlotsController(IAvailabilityService service, ILookupService lookup)
        {
            _service = service;
            _lookup = lookup;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var slot = await _service.GetAsync(id);
            if (slot == null) return NotFound();
            return Ok(slot);
        }

        [HttpPost]
        public async Task<IActionResult> Create(AvailabilitySlot slot)
        {
            if (!await _lookup.IsValidDay(slot.DayId))
                return BadRequest("Invalid day_id");

            var created = await _service.CreateAsync(slot);
            return CreatedAtAction(nameof(Get), new { id = created.AvailabilityId }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, AvailabilitySlot updated)
        {
            var slot = await _service.GetAsync(id);
            if (slot == null) return NotFound();

            if (!await _lookup.IsValidDay(updated.DayId))
                return BadRequest("Invalid day_id");

            await _service.UpdateAsync(slot, updated);
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
