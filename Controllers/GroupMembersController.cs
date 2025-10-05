using Microsoft.AspNetCore.Mvc;
using StudyGroup.Repository.Models;
using StudyGroup.Service.Models.Services;
using StudyGroup.Service.Interface;


[Route("api/[controller]")]
[ApiController]
public class GroupMembersController : ControllerBase
{
    private readonly IGroupMemberService _service;

    public GroupMembersController(IGroupMemberService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var member = await _service.GetAsync(id);
        if (member == null) return NotFound();
        return Ok(member);
    }

    [HttpPost]
    public async Task<IActionResult> Create(GroupMember member)
    {
        try
        {
            var created = await _service.CreateAsync(member);
            return CreatedAtAction(nameof(Get), new { id = created.GroupMemberId }, created);
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, GroupMember updated)
    {
        var member = await _service.GetAsync(id);
        if (member == null) return NotFound();

        member.StatusId = updated.StatusId;

        try
        {
            await _service.UpdateAsync(member);
            return NoContent();
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
