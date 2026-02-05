using Microsoft.AspNetCore.Mvc;
using StudyGroup.Service.Models.Services;
using StudyGroup.Repository.Models;

using System.Threading.Tasks;

namespace StudyGroup.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseScraperController : ControllerBase
    {
        private readonly CourseScraperService _scraper;

        public CourseScraperController(CourseScraperService scraper)
        {
            _scraper = scraper;
        }

        // POST: api/coursescraper/run
        [HttpPost("run")]
        public async Task<IActionResult> RunScraper()
        {
            await _scraper.ScrapeAndPopulateCoursesAsync();
            return Ok("Courses scraped successfully.");
        }


    }
}
