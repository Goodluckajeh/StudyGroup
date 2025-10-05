using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudyGroup.Repository.Data;

[Route("api/[controller]")]
[ApiController]
public class MembershipStatusController : ControllerBase
{
    private readonly AppDbContext _context;
    public MembershipStatusController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _context.MembershipStatuses.ToListAsync());
}
