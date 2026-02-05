using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StudyGroup.Service.Services;
using System.Threading.Tasks;

namespace StudyGroup.Api.Controllers
{
    /// <summary>
    /// Internal API controller for course scraping operations.
    /// Provides endpoints to scrape courses from Principia College catalog and populate the database.
    /// HIDDEN FROM SWAGGER - Internal use only.
    /// Requires authentication for security purposes.
    /// </summary>
    [ApiController]
    [Route("api/internal/[controller]")]
    [Authorize] // Require authentication for scraping operations
    [ApiExplorerSettings(IgnoreApi = true)] // Hide from Swagger documentation
    public class CourseScrapingController : ControllerBase
    {
        private readonly CourseScrapingService _scrapingService;

        /// <summary>
        /// Initializes a new instance of the CourseScrapingController.
        /// </summary>
        /// <param name="scrapingService">The course scraping service</param>
        public CourseScrapingController(CourseScrapingService scrapingService)
        {
            _scrapingService = scrapingService;
        }

        /// <summary>
        /// POST: api/internal/coursescraping/run
        /// Runs the course scraping process to populate the database with courses from Principia College catalog.
        /// This operation may take several minutes to complete.
        /// AUTHENTICATION REQUIRED: Only authenticated users can run scraping operations.
        /// HIDDEN FROM SWAGGER: Internal endpoint not visible to public users.
        /// 
        /// Example usage:
        /// POST /api/internal/coursescraping/run
        /// Authorization: Bearer {jwt-token}
        /// </summary>
        /// <returns>Detailed scraping results including statistics and any errors</returns>
        [HttpPost("run")]
        public async Task<IActionResult> RunCourseScraping()
        {
            try
            {
                var result = await _scrapingService.ScrapeAndPopulateCoursesAsync();

                if (result.IsSuccess)
                {
                    return Ok(new
                    {
                        Message = "?? Course scraping completed successfully!",
                        Result = new
                        {
                            Success = result.IsSuccess,
                            Duration = $"{result.Duration.TotalSeconds:F1} seconds",
                            Statistics = new
                            {
                                TotalCoursesFound = result.TotalCoursesFound,
                                CoursesAdded = result.CoursesAdded,
                                CoursesSkipped = result.CoursesSkipped,
                                ErrorCount = result.Errors.Count
                            }
                        },
                        Details = new
                        {
                            StartTime = result.StartTime,
                            EndTime = result.EndTime,
                            Messages = result.Messages.Take(10).ToList(), // Show first 10 messages
                            Errors = result.Errors,
                            Note = result.Messages.Count > 10 ? $"Showing first 10 of {result.Messages.Count} messages" : null
                        },
                        NextSteps = new[]
                        {
                            "Use GET /api/courses to view all scraped courses",
                            "Use GET /api/internal/coursescraping/statistics for database statistics",
                            "Create study groups using the new course data"
                        }
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        Message = "? Course scraping failed",
                        Error = "Scraping operation completed with errors",
                        Details = new
                        {
                            Duration = $"{result.Duration.TotalSeconds:F1} seconds",
                            Errors = result.Errors,
                            Messages = result.Messages,
                            PartialResults = new
                            {
                                CoursesFound = result.TotalCoursesFound,
                                CoursesAdded = result.CoursesAdded,
                                CoursesSkipped = result.CoursesSkipped
                            }
                        },
                        Recommendation = "Check the error details and try again"
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "?? Course scraping failed with unexpected error",
                    Error = ex.Message,
                    Note = "Please check the server logs for more details"
                });
            }
        }

        /// <summary>
        /// GET: api/internal/coursescraping/statistics
        /// Gets current course database statistics and scraping status.
        /// Shows how many courses are in the database and their quality.
        /// AUTHENTICATION REQUIRED: Statistics are only available to authenticated users.
        /// HIDDEN FROM SWAGGER: Internal endpoint not visible to public users.
        /// </summary>
        /// <returns>Database statistics and course information</returns>
        [HttpGet("statistics")]
        public async Task<IActionResult> GetScrapingStatistics()
        {
            try
            {
                var statistics = await _scrapingService.GetScrapingStatisticsAsync();

                return Ok(new
                {
                    Message = "?? Course Database Statistics",
                    Statistics = new
                    {
                        TotalCourses = statistics.TotalCoursesInDatabase,
                        CoursesWithCodes = statistics.CoursesWithCodes,
                        CoursesWithDescriptions = statistics.CoursesWithDescriptions,
                        LastUpdated = statistics.LastUpdated
                    },
                    DataQuality = new
                    {
                        CodeCoverage = statistics.TotalCoursesInDatabase > 0 ? 
                            $"{(statistics.CoursesWithCodes * 100.0 / statistics.TotalCoursesInDatabase):F1}%" : "0%",
                        DescriptionCoverage = statistics.TotalCoursesInDatabase > 0 ? 
                            $"{(statistics.CoursesWithDescriptions * 100.0 / statistics.TotalCoursesInDatabase):F1}%" : "0%",
                        Status = statistics.TotalCoursesInDatabase > 0 ? "Database populated" : "No courses found"
                    },
                    SampleCourses = statistics.SampleCourses,
                    Actions = new[]
                    {
                        "Use POST /api/internal/coursescraping/run to scrape more courses",
                        "Use GET /api/courses to view all courses",
                        "Use the courses to create study groups"
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "Failed to get scraping statistics",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// GET: api/internal/coursescraping/test
        /// Tests the scraping service connection and basic functionality.
        /// Performs a quick test without actually scraping data.
        /// AUTHENTICATION REQUIRED: Testing endpoint for authenticated users only.
        /// HIDDEN FROM SWAGGER: Internal endpoint not visible to public users.
        /// </summary>
        /// <returns>Connection test results</returns>
        [HttpGet("test")]
        public async Task<IActionResult> TestScrapingConnection()
        {
            try
            {
                // Simple test to verify the scraping service is working
                var statistics = await _scrapingService.GetScrapingStatisticsAsync();
                
                return Ok(new
                {
                    Message = "? Course scraping service is operational",
                    Test = new
                    {
                        ServiceStatus = "Active",
                        DatabaseConnection = "Working",
                        CurrentCourseCount = statistics.TotalCoursesInDatabase
                    },
                    ReadyToScrape = true,
                    Note = "Use POST /api/internal/coursescraping/run to start scraping courses"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "? Course scraping service test failed",
                    Error = ex.Message,
                    ServiceStatus = "Error",
                    Note = "Please check service configuration and database connection"
                });
            }
        }
    }
}