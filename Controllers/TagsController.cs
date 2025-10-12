using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StudyGroup.Service.Interfaces;
using StudyGroup.Service.DTOs;
using StudyGroup.Api.Services;

namespace StudyGroup.Api.Controllers
{
    /// <summary>
    /// API controller for Tag endpoints with clean CRUD operations and duplicate prevention.
    /// Tags are automatically created when users and study groups are created/updated.
    /// This controller provides manual tag management and querying capabilities.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Require JWT authentication for all endpoints
    public class TagsController : ControllerBase
    {
        private readonly ITagService _service;
        private readonly AutoTaggingService _autoTaggingService;

        /// <summary>
        /// Initializes a new instance of the TagsController.
        /// </summary>
        /// <param name="service">The tag service for handling business logic</param>
        /// <param name="autoTaggingService">The auto-tagging service for summary operations</param>
        public TagsController(ITagService service, AutoTaggingService autoTaggingService)
        {
            _service = service;
            _autoTaggingService = autoTaggingService;
        }

        #region Standard CRUD Operations

        /// <summary>
        /// GET: api/tags
        /// Retrieves all tags from the system with duplicate prevention summary.
        /// Shows both user skill tags and study group course tags.
        /// </summary>
        /// <returns>List of all tags with their entity associations and duplicate stats</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tags = await _service.GetAllTagsAsync();
            var tagList = tags.ToList();
            
            // Check for any potential duplicates (shouldn't exist with our system)
            var userTags = tagList.Where(t => t.EntityTypeId == 1).ToList();
            var studyGroupTags = tagList.Where(t => t.EntityTypeId == 4).ToList();
            
            var userDuplicates = userTags.GroupBy(t => new { t.EntityId, Name = t.Name.ToLowerInvariant() })
                                        .Where(g => g.Count() > 1)
                                        .ToList();
                                        
            var studyGroupDuplicates = studyGroupTags.GroupBy(t => new { t.EntityId, Name = t.Name.ToLowerInvariant() })
                                                    .Where(g => g.Count() > 1)
                                                    .ToList();
            
            return Ok(new
            {
                Tags = tagList,
                Count = tagList.Count,
                Summary = new
                {
                    UserSkillTags = userTags.Count,
                    StudyGroupCourseTags = studyGroupTags.Count,
                    Total = tagList.Count
                },
                DuplicateCheck = new
                {
                    UserDuplicatesFound = userDuplicates.Count,
                    StudyGroupDuplicatesFound = studyGroupDuplicates.Count,
                    SystemStatus = (userDuplicates.Count == 0 && studyGroupDuplicates.Count == 0) ? 
                        "? No duplicates found - system working correctly" : 
                        "?? Duplicates detected - may need cleanup",
                    Note = "The auto-tagging system prevents duplicates automatically"
                }
            });
        }

        /// <summary>
        /// GET: api/tags/{id}
        /// Retrieves a specific tag by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the tag</param>
        /// <returns>The tag details if found, otherwise NotFound</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var tag = await _service.GetTagByIdAsync(id);
            if (tag == null) return NotFound();
            return Ok(tag);
        }

        /// <summary>
        /// POST: api/tags
        /// Creates a new tag manually (rarely needed - tags are usually created automatically).
        /// </summary>
        /// <param name="tagDto">The tag data including entity association</param>
        /// <returns>The created tag with its new ID</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTagDto tagDto)
        {
            try
            {
                // Check if this would create a duplicate
                var existingTags = await _service.GetTagsByEntityAsync(tagDto.EntityTypeId, tagDto.EntityId);
                var duplicate = existingTags.FirstOrDefault(t => 
                    t.Name.Equals(tagDto.Name, StringComparison.OrdinalIgnoreCase));
                
                if (duplicate != null)
                {
                    return BadRequest(new 
                    { 
                        Error = "Duplicate tag prevention", 
                        Message = $"Tag '{tagDto.Name}' already exists for this entity",
                        ExistingTagId = duplicate.TagId,
                        Note = "The system prevents duplicate tags automatically"
                    });
                }

                var id = await _service.CreateTagAsync(tagDto);
                return CreatedAtAction(nameof(GetById), new { id }, new
                {
                    TagId = id,
                    Message = "Tag created successfully",
                    Note = "?? Tip: Tags are usually created automatically when users add skills or study groups specify courses"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = "Failed to create tag", Details = ex.Message });
            }
        }

        /// <summary>
        /// PUT: api/tags/{id}
        /// Updates an existing tag.
        /// </summary>
        /// <param name="id">The ID of the tag to update</param>
        /// <param name="tagDto">The updated tag data</param>
        /// <returns>NoContent if successful, NotFound if tag doesn't exist</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTagDto tagDto)
        {
            var success = await _service.UpdateTagAsync(id, tagDto);
            if (!success) return NotFound();
            return NoContent();
        }

        /// <summary>
        /// DELETE: api/tags/{id}
        /// Removes a tag by its ID.
        /// Note: Tags are automatically removed when associated users/study groups are deleted.
        /// </summary>
        /// <param name="id">The ID of the tag to delete</param>
        /// <returns>NoContent if successful, NotFound if tag doesn't exist</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.DeleteTagAsync(id);
            if (!success) return NotFound();
            return Ok(new 
            { 
                Message = "Tag deleted successfully",
                Note = "?? Tags are automatically managed when users update skills or study groups update courses"
            });
        }

        #endregion

        #region Entity-Specific Tag Queries with Duplicate Checking

        /// <summary>
        /// GET: api/tags/users/{userId}
        /// Gets all skill tags for a specific user with duplicate detection.
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <returns>List of skill tags associated with the user</returns>
        [HttpGet("users/{userId}")]
        public async Task<IActionResult> GetUserSkillTags(int userId)
        {
            var tags = await _service.GetTagsByEntityAsync(1, userId); // EntityType 1 = User
            var tagList = tags.ToList();
            
            // Check for duplicates (shouldn't exist)
            var duplicates = tagList.GroupBy(t => t.Name.ToLowerInvariant())
                                   .Where(g => g.Count() > 1)
                                   .Select(g => new { SkillName = g.Key, Count = g.Count() })
                                   .ToList();
            
            return Ok(new
            {
                UserId = userId,
                EntityType = "User Skills",
                Tags = tagList,
                Count = tagList.Count,
                SkillNames = tagList.Select(t => t.Name).ToList(),
                DuplicateCheck = new
                {
                    DuplicatesFound = duplicates.Count,
                    Duplicates = duplicates,
                    Status = duplicates.Count == 0 ? "? No duplicates" : "?? Duplicates detected"
                }
            });
        }

        /// <summary>
        /// GET: api/tags/studygroups/{groupId}
        /// Gets all course tags for a specific study group with duplicate detection.
        /// </summary>
        /// <param name="groupId">The study group ID</param>
        /// <returns>List of course tags associated with the study group</returns>
        [HttpGet("studygroups/{groupId}")]
        public async Task<IActionResult> GetStudyGroupCourseTags(int groupId)
        {
            var tags = await _service.GetTagsByEntityAsync(4, groupId); // EntityType 4 = StudyGroup
            var tagList = tags.ToList();
            
            // Check for duplicates (shouldn't exist)
            var duplicates = tagList.GroupBy(t => t.Name.ToLowerInvariant())
                                   .Where(g => g.Count() > 1)
                                   .Select(g => new { TopicName = g.Key, Count = g.Count() })
                                   .ToList();
            
            return Ok(new
            {
                StudyGroupId = groupId,
                EntityType = "Study Group Course Tags",
                Tags = tagList,
                Count = tagList.Count,
                CourseTopics = tagList.Select(t => t.Name).ToList(),
                DuplicateCheck = new
                {
                    DuplicatesFound = duplicates.Count,
                    Duplicates = duplicates,
                    Status = duplicates.Count == 0 ? "? No duplicates" : "?? Duplicates detected"
                }
            });
        }

        /// <summary>
        /// GET: api/tags/users/{userId}/summary
        /// Gets a detailed summary of a user's tags using AutoTaggingService.
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <returns>Detailed tag summary from AutoTaggingService</returns>
        [HttpGet("users/{userId}/summary")]
        public async Task<IActionResult> GetUserTagSummary(int userId)
        {
            var summary = await _autoTaggingService.GetUserTagSummaryAsync(userId);
            return Ok(summary);
        }

        /// <summary>
        /// GET: api/tags/studygroups/{groupId}/summary
        /// Gets a detailed summary of a study group's tags using AutoTaggingService.
        /// </summary>
        /// <param name="groupId">The study group ID</param>
        /// <returns>Detailed tag summary from AutoTaggingService</returns>
        [HttpGet("studygroups/{groupId}/summary")]
        public async Task<IActionResult> GetStudyGroupTagSummary(int groupId)
        {
            var summary = await _autoTaggingService.GetStudyGroupTagSummaryAsync(groupId);
            return Ok(summary);
        }

        #endregion

        #region System Health and Statistics

        /// <summary>
        /// GET: api/tags/stats
        /// Gets comprehensive statistics about the tagging system including duplicate health check.
        /// </summary>
        /// <returns>Detailed tagging system statistics and health</returns>
        [HttpGet("stats")]
        public async Task<IActionResult> GetTagStats()
        {
            var allTags = await _service.GetAllTagsAsync();
            var tagList = allTags.ToList();
            
            var userTags = tagList.Where(t => t.EntityTypeId == 1).ToList();
            var studyGroupTags = tagList.Where(t => t.EntityTypeId == 4).ToList();
            
            // Comprehensive duplicate analysis
            var userDuplicates = userTags.GroupBy(t => new { t.EntityId, Name = t.Name.ToLowerInvariant() })
                                        .Where(g => g.Count() > 1)
                                        .ToList();
                                        
            var studyGroupDuplicates = studyGroupTags.GroupBy(t => new { t.EntityId, Name = t.Name.ToLowerInvariant() })
                                                    .Where(g => g.Count() > 1)
                                                    .ToList();
            
            return Ok(new
            {
                Message = "??? Tag System Statistics & Health Check",
                OverallStats = new
                {
                    TotalTags = tagList.Count,
                    UserSkillTags = userTags.Count,
                    StudyGroupCourseTags = studyGroupTags.Count
                },
                SkillStats = new
                {
                    UniqueSkills = userTags.Select(t => t.Name).Distinct().Count(),
                    UsersWithSkillTags = userTags.Select(t => t.EntityId).Distinct().Count(),
                    MostPopularSkills = userTags.GroupBy(t => t.Name)
                                                .OrderByDescending(g => g.Count())
                                                .Take(10)
                                                .Select(g => new { Skill = g.Key, Count = g.Count() })
                                                .ToList()
                },
                CourseStats = new
                {
                    UniqueCourseTopics = studyGroupTags.Select(t => t.Name).Distinct().Count(),
                    StudyGroupsWithTags = studyGroupTags.Select(t => t.EntityId).Distinct().Count(),
                    MostPopularTopics = studyGroupTags.GroupBy(t => t.Name)
                                                     .OrderByDescending(g => g.Count())
                                                     .Take(10)
                                                     .Select(g => new { Topic = g.Key, Count = g.Count() })
                                                     .ToList()
                },
                SystemHealth = new
                {
                    DuplicatePreventionStatus = "? Active",
                    UserTagDuplicates = userDuplicates.Count,
                    StudyGroupTagDuplicates = studyGroupDuplicates.Count,
                    OverallHealth = (userDuplicates.Count == 0 && studyGroupDuplicates.Count == 0) ? 
                        "? Excellent - No duplicates detected" : 
                        "?? Issues detected - Duplicates found",
                    AutoTaggingEnabled = true,
                    Note = "Tags are automatically created/updated with duplicate prevention when users modify skills or study groups change courses"
                }
            });
        }

        #endregion
    }
}
