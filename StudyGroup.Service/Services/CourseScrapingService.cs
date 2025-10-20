using StudyGroup.Data.Interfaces;
using StudyGroup.Data.Models;
using StudyGroup.Service.Interfaces;
using StudyGroup.Service.DTOs;
using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace StudyGroup.Service.Services
{
    /// <summary>
    /// Service for scraping courses from Principia College catalog.
    /// Implements web scraping to populate the course database with real course data.
    /// Integrates with existing CourseService for data management.
    /// </summary>
    public class CourseScrapingService
    {
        private readonly ICourseRepository _courseRepository;
        private readonly ICourseService _courseService;

        /// <summary>
        /// Initializes a new instance of the CourseScrapingService.
        /// </summary>
        /// <param name="courseRepository">Repository for direct course database access</param>
        /// <param name="courseService">Service for course business logic</param>
        public CourseScrapingService(ICourseRepository courseRepository, ICourseService courseService)
        {
            _courseRepository = courseRepository;
            _courseService = courseService;
        }

        /// <summary>
        /// Scrapes courses from Principia College catalog and populates the database.
        /// Avoids duplicates by checking existing course codes before insertion.
        /// </summary>
        /// <returns>Scraping results including success status and course counts</returns>
        public async Task<CourseScrapingResult> ScrapeAndPopulateCoursesAsync()
        {
            var result = new CourseScrapingResult
            {
                StartTime = DateTime.UtcNow,
                IsSuccess = false,
                TotalCoursesFound = 0,
                CoursesAdded = 0,
                CoursesSkipped = 0,
                Errors = new List<string>()
            };

            try
            {
                var baseUrl = "https://catalog.principiacollege.edu";
                var catalogUrl = $"{baseUrl}/courses-instruction/courses/";

                var web = new HtmlWeb();
                HtmlDocument doc;

                try
                {
                    doc = web.Load(catalogUrl);
                    result.Messages.Add("Successfully loaded main catalog page.");
                }
                catch (Exception ex)
                {
                    result.Errors.Add($"Failed to load catalog page: {ex.Message}");
                    result.EndTime = DateTime.UtcNow;
                    return result;
                }

                // Step 1: Extract all course group links
                var groupLinks = doc.DocumentNode.SelectNodes("//ul/li/a[@href]");
                if (groupLinks == null || !groupLinks.Any())
                {
                    result.Errors.Add("No course groups found on the catalog page.");
                    result.EndTime = DateTime.UtcNow;
                    return result;
                }

                result.Messages.Add($"Found {groupLinks.Count} course groups to process.");

                // Step 2: Process each course group
                foreach (var link in groupLinks)
                {
                    var groupHref = link.GetAttributeValue("href", "").Trim();
                    if (string.IsNullOrWhiteSpace(groupHref))
                        continue;

                    var groupUrl = baseUrl + groupHref;
                    var groupResult = await ProcessCourseGroupAsync(web, groupUrl);
                    
                    result.TotalCoursesFound += groupResult.CoursesFound;
                    result.CoursesAdded += groupResult.CoursesAdded;
                    result.CoursesSkipped += groupResult.CoursesSkipped;
                    result.Messages.AddRange(groupResult.Messages);
                    result.Errors.AddRange(groupResult.Errors);
                }

                result.IsSuccess = true;
                result.Messages.Add($"Scraping completed successfully. Total: {result.TotalCoursesFound} found, {result.CoursesAdded} added, {result.CoursesSkipped} skipped.");
            }
            catch (Exception ex)
            {
                result.Errors.Add($"Scraping failed with exception: {ex.Message}");
                result.IsSuccess = false;
            }
            finally
            {
                result.EndTime = DateTime.UtcNow;
                result.Duration = result.EndTime - result.StartTime;
            }

            return result;
        }

        /// <summary>
        /// Processes a single course group page and extracts all courses.
        /// </summary>
        /// <param name="web">HtmlWeb instance for loading pages</param>
        /// <param name="groupUrl">URL of the course group page</param>
        /// <returns>Processing results for this group</returns>
        private async Task<GroupProcessingResult> ProcessCourseGroupAsync(HtmlWeb web, string groupUrl)
        {
            var result = new GroupProcessingResult
            {
                GroupUrl = groupUrl,
                CoursesFound = 0,
                CoursesAdded = 0,
                CoursesSkipped = 0,
                Messages = new List<string>(),
                Errors = new List<string>()
            };

            try
            {
                // Load the group page
                var groupDoc = web.Load(groupUrl);
                result.Messages.Add($"Loaded group page: {groupUrl}");

                // Extract all course blocks from the page
                var courseNodes = groupDoc.DocumentNode.SelectNodes("//div[contains(@class,'courseblock')]");
                if (courseNodes == null || !courseNodes.Any())
                {
                    result.Messages.Add($"No courses found on page: {groupUrl}");
                    return result;
                }

                result.CoursesFound = courseNodes.Count;
                result.Messages.Add($"Found {result.CoursesFound} courses in this group.");

                // Process each course
                foreach (var node in courseNodes)
                {
                    try
                    {
                        var courseData = ExtractCourseData(node);
                        if (courseData == null)
                        {
                            result.Messages.Add("Skipped course with incomplete data.");
                            continue;
                        }

                        // Check if course already exists
                        var existingCourse = await _courseRepository.ExistsByCodeAsync(courseData.CourseCode);
                        if (existingCourse)
                        {
                            result.CoursesSkipped++;
                            result.Messages.Add($"Skipped duplicate course: {courseData.CourseCode}");
                            continue;
                        }

                        // Create new course
                        var courseId = await _courseService.CreateCourseAsync(courseData);
                        result.CoursesAdded++;
                        result.Messages.Add($"Added course: {courseData.CourseCode} - {courseData.CourseName}");
                    }
                    catch (Exception ex)
                    {
                        result.Errors.Add($"Failed to process course: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                result.Errors.Add($"Failed to load group page {groupUrl}: {ex.Message}");
            }

            return result;
        }

        /// <summary>
        /// Extracts course data from an HTML course block node.
        /// </summary>
        /// <param name="courseNode">HTML node containing course information</param>
        /// <returns>CreateCourseDto with extracted data, or null if extraction fails</returns>
        private CreateCourseDto? ExtractCourseData(HtmlNode courseNode)
        {
            try
            {
                // Extract course code
                var codeNode = courseNode.SelectSingleNode(".//span[contains(@class,'coursecode')]");
                if (codeNode == null) return null;
                var courseCode = CleanText(codeNode.InnerText);

                // Extract course title
                var titleNode = courseNode.SelectSingleNode(".//span[contains(@class,'coursetitle')]");
                if (titleNode == null) return null;
                var courseName = CleanText(titleNode.InnerText);

                // Extract course description (optional)
                var descNode = courseNode.SelectSingleNode(".//p[contains(@class,'courseblockdesc')]");
                var description = descNode != null ? CleanText(descNode.InnerText) : null;

                // Validate extracted data
                if (string.IsNullOrWhiteSpace(courseCode) || string.IsNullOrWhiteSpace(courseName))
                    return null;

                return new CreateCourseDto
                {
                    CourseCode = courseCode,
                    CourseName = courseName,
                    Description = description
                };
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Cleans and normalizes text extracted from HTML.
        /// </summary>
        /// <param name="text">Raw text from HTML</param>
        /// <returns>Cleaned text</returns>
        private string CleanText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            // Remove HTML entities and extra whitespace
            text = HtmlEntity.DeEntitize(text);
            text = Regex.Replace(text, @"\s+", " ");
            return text.Trim();
        }

        /// <summary>
        /// Gets scraping statistics and status.
        /// </summary>
        /// <returns>Current scraping statistics</returns>
        public async Task<CourseScrapingStatistics> GetScrapingStatisticsAsync()
        {
            var allCourses = await _courseRepository.GetAllAsync();
            var coursesList = allCourses.ToList();

            return new CourseScrapingStatistics
            {
                TotalCoursesInDatabase = coursesList.Count,
                CoursesWithCodes = coursesList.Count(c => !string.IsNullOrWhiteSpace(c.CourseCode)),
                CoursesWithDescriptions = coursesList.Count(c => !string.IsNullOrWhiteSpace(c.Description)),
                LastUpdated = DateTime.UtcNow,
                SampleCourses = coursesList.Take(5).Select(c => $"{c.CourseCode} - {c.CourseName}").ToList()
            };
        }
    }

    #region Result Models

    /// <summary>
    /// Represents the result of a course scraping operation.
    /// </summary>
    public class CourseScrapingResult
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan Duration { get; set; }
        public bool IsSuccess { get; set; }
        public int TotalCoursesFound { get; set; }
        public int CoursesAdded { get; set; }
        public int CoursesSkipped { get; set; }
        public List<string> Messages { get; set; } = new List<string>();
        public List<string> Errors { get; set; } = new List<string>();
    }

    /// <summary>
    /// Represents the result of processing a single course group.
    /// </summary>
    public class GroupProcessingResult
    {
        public string GroupUrl { get; set; } = string.Empty;
        public int CoursesFound { get; set; }
        public int CoursesAdded { get; set; }
        public int CoursesSkipped { get; set; }
        public List<string> Messages { get; set; } = new List<string>();
        public List<string> Errors { get; set; } = new List<string>();
    }

    /// <summary>
    /// Represents scraping statistics and database status.
    /// </summary>
    public class CourseScrapingStatistics
    {
        public int TotalCoursesInDatabase { get; set; }
        public int CoursesWithCodes { get; set; }
        public int CoursesWithDescriptions { get; set; }
        public DateTime LastUpdated { get; set; }
        public List<string> SampleCourses { get; set; } = new List<string>();
    }

    #endregion
}