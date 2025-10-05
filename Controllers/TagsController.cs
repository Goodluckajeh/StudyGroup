using Microsoft.AspNetCore.Mvc;
using StudyGroup.Repository.Models;
using StudyGroup.Service.Interface;


[Route("api/[controller]")]
[ApiController]
public class TagsController : ControllerBase
{
    private readonly ITagService _service;
    private readonly ILookupService _lookup;

    public TagsController(ITagService service, ILookupService lookup)
    {
        _service = service;
        _lookup = lookup;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var tag = await _service.GetAsync(id);
        if (tag == null) return NotFound();
        return Ok(tag);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Tag tag)
    {
        if (!await _lookup.IsValidEntityType(tag.EntityTypeId))
            return BadRequest("Invalid entity_type_id");

        var created = await _service.CreateAsync(tag);
        return CreatedAtAction(nameof(Get), new { id = created.TagId }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Tag updated)
    {
        var tag = await _service.GetAsync(id);
        if (tag == null) return NotFound();

        if (!await _lookup.IsValidEntityType(updated.EntityTypeId))
            return BadRequest("Invalid entity_type_id");

        await _service.UpdateAsync(tag, updated);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
