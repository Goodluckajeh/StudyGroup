using StudyGroup.Service.Interfaces;
using StudyGroup.Service.DTOs;

namespace StudyGroup.Api.Services
{
    /// <summary>
    /// Service for automatically creating tags when users and study groups are created or updated.
    /// Prevents duplicate tags for both users and study groups, ensuring clean tag management.
    /// </summary>
    public class AutoTaggingService
    {
        private readonly ITagService _tagService;

        public AutoTaggingService(ITagService tagService)
        {
            _tagService = tagService;
        }

        /// <summary>
        /// Creates skill tags for a user, avoiding duplicates.
        /// Called automatically when users are created or updated.
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="skills">Comma-separated skills string</param>
        /// <returns>Number of new tags created</returns>
        public async Task<int> CreateUserSkillTagsAsync(int userId, string? skills)
        {
            if (string.IsNullOrWhiteSpace(skills))
                return 0;

            // Get existing tags for this user to avoid duplicates
            var existingTags = await _tagService.GetTagsByEntityAsync(1, userId); // EntityType 1 = User
            var existingTagNames = existingTags.Select(t => t.Name.ToLowerInvariant()).ToHashSet();

            var createdCount = 0;
            var skillList = ParseSkills(skills);

            foreach (var skill in skillList)
            {
                var skillLower = skill.Trim().ToLowerInvariant();
                
                // Skip if tag already exists for this user
                if (existingTagNames.Contains(skillLower))
                    continue;

                try
                {
                    var createTagDto = new CreateTagDto
                    {
                        Name = skill.Trim(),
                        EntityTypeId = 1, // User entity type
                        EntityId = userId
                    };

                    await _tagService.CreateTagAsync(createTagDto);
                    createdCount++;
                }
                catch (Exception)
                {
                    // Tag creation failed - continue with next skill
                }
            }

            return createdCount;
        }

        /// <summary>
        /// Creates course-related tags for a study group, avoiding duplicates.
        /// Called automatically when study groups are created or updated.
        /// </summary>
        /// <param name="studyGroupId">The study group ID</param>
        /// <param name="courseName">The course name</param>
        /// <returns>Number of new tags created</returns>
        public async Task<int> CreateStudyGroupCourseTagsAsync(int studyGroupId, string courseName)
        {
            if (string.IsNullOrWhiteSpace(courseName))
                return 0;

            // Get existing tags for this study group to avoid duplicates
            var existingTags = await _tagService.GetTagsByEntityAsync(4, studyGroupId); // EntityType 4 = StudyGroup
            var existingTagNames = existingTags.Select(t => t.Name.ToLowerInvariant()).ToHashSet();

            var createdCount = 0;
            var courseTags = GenerateCourseRelatedTags(courseName);

            foreach (var tag in courseTags)
            {
                var tagLower = tag.Trim().ToLowerInvariant();
                
                // Skip if tag already exists for this study group
                if (existingTagNames.Contains(tagLower))
                    continue;

                try
                {
                    var createTagDto = new CreateTagDto
                    {
                        Name = tag.Trim(),
                        EntityTypeId = 4, // StudyGroup entity type
                        EntityId = studyGroupId
                    };

                    await _tagService.CreateTagAsync(createTagDto);
                    createdCount++;
                }
                catch (Exception)
                {
                    // Tag creation failed - continue with next tag
                }
            }

            return createdCount;
        }

        /// <summary>
        /// Updates skill tags for a user by removing old ones and creating new ones.
        /// Ensures no duplicates and clean tag management.
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="newSkills">The updated skills string</param>
        /// <returns>Number of new tags created</returns>
        public async Task<int> UpdateUserSkillTagsAsync(int userId, string? newSkills)
        {
            // Remove all existing skill tags for this user first
            await RemoveUserSkillTagsAsync(userId);

            // Create new skill tags (no duplicates possible since we just cleared them)
            return await CreateUserSkillTagsAsync(userId, newSkills);
        }

        /// <summary>
        /// Updates course tags for a study group by removing old ones and creating new ones.
        /// Ensures no duplicates and clean tag management.
        /// </summary>
        /// <param name="studyGroupId">The study group ID</param>
        /// <param name="newCourseName">The updated course name</param>
        /// <returns>Number of new tags created</returns>
        public async Task<int> UpdateStudyGroupCourseTagsAsync(int studyGroupId, string newCourseName)
        {
            // Remove all existing course tags for this study group first
            await RemoveStudyGroupCourseTagsAsync(studyGroupId);

            // Create new course tags (no duplicates possible since we just cleared them)
            return await CreateStudyGroupCourseTagsAsync(studyGroupId, newCourseName);
        }

        /// <summary>
        /// Adds additional tags to a study group without removing existing ones.
        /// This is useful when you want to append tags rather than replace them.
        /// Still prevents duplicates.
        /// </summary>
        /// <param name="studyGroupId">The study group ID</param>
        /// <param name="additionalTags">Additional tags to add</param>
        /// <returns>Number of new tags created</returns>
        public async Task<int> AddAdditionalStudyGroupTagsAsync(int studyGroupId, string[] additionalTags)
        {
            if (additionalTags == null || !additionalTags.Any())
                return 0;

            // Get existing tags for this study group to avoid duplicates
            var existingTags = await _tagService.GetTagsByEntityAsync(4, studyGroupId);
            var existingTagNames = existingTags.Select(t => t.Name.ToLowerInvariant()).ToHashSet();

            var createdCount = 0;

            foreach (var tag in additionalTags)
            {
                if (string.IsNullOrWhiteSpace(tag))
                    continue;

                var tagLower = tag.Trim().ToLowerInvariant();
                
                // Skip if tag already exists for this study group
                if (existingTagNames.Contains(tagLower))
                    continue;

                try
                {
                    var createTagDto = new CreateTagDto
                    {
                        Name = tag.Trim(),
                        EntityTypeId = 4, // StudyGroup entity type
                        EntityId = studyGroupId
                    };

                    await _tagService.CreateTagAsync(createTagDto);
                    createdCount++;
                    
                    // Add to existing set to prevent duplicates in this batch
                    existingTagNames.Add(tagLower);
                }
                catch (Exception)
                {
                    // Tag creation failed - continue with next tag
                }
            }

            return createdCount;
        }

        /// <summary>
        /// Removes all skill tags for a specific user.
        /// </summary>
        /// <param name="userId">The user ID</param>
        public async Task RemoveUserSkillTagsAsync(int userId)
        {
            try
            {
                var userTags = await _tagService.GetTagsByEntityAsync(1, userId); // EntityType 1 = User
                foreach (var tag in userTags)
                {
                    await _tagService.DeleteTagAsync(tag.TagId);
                }
            }
            catch (Exception)
            {
                // Continue even if some tags can't be deleted
            }
        }

        /// <summary>
        /// Removes all course tags for a specific study group.
        /// </summary>
        /// <param name="studyGroupId">The study group ID</param>
        public async Task RemoveStudyGroupCourseTagsAsync(int studyGroupId)
        {
            try
            {
                var groupTags = await _tagService.GetTagsByEntityAsync(4, studyGroupId); // EntityType 4 = StudyGroup
                foreach (var tag in groupTags)
                {
                    await _tagService.DeleteTagAsync(tag.TagId);
                }
            }
            catch (Exception)
            {
                // Continue even if some tags can't be deleted
            }
        }

        /// <summary>
        /// Gets a summary of tags for a user (useful for debugging or display).
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <returns>Summary of user's skill tags</returns>
        public async Task<object> GetUserTagSummaryAsync(int userId)
        {
            try
            {
                var userTags = await _tagService.GetTagsByEntityAsync(1, userId);
                var tagList = userTags.ToList();
                
                return new
                {
                    UserId = userId,
                    TotalTags = tagList.Count,
                    SkillTags = tagList.Select(t => t.Name).ToList(),
                    Note = "These tags make the user discoverable by study groups with matching course topics"
                };
            }
            catch (Exception)
            {
                return new { UserId = userId, Error = "Unable to retrieve tag summary" };
            }
        }

        /// <summary>
        /// Gets a summary of tags for a study group (useful for debugging or display).
        /// </summary>
        /// <param name="studyGroupId">The study group ID</param>
        /// <returns>Summary of study group's course tags</returns>
        public async Task<object> GetStudyGroupTagSummaryAsync(int studyGroupId)
        {
            try
            {
                var groupTags = await _tagService.GetTagsByEntityAsync(4, studyGroupId);
                var tagList = groupTags.ToList();
                
                return new
                {
                    StudyGroupId = studyGroupId,
                    TotalTags = tagList.Count,
                    CourseTags = tagList.Select(t => t.Name).ToList(),
                    Note = "These tags make the study group discoverable by users with matching skills"
                };
            }
            catch (Exception)
            {
                return new { StudyGroupId = studyGroupId, Error = "Unable to retrieve tag summary" };
            }
        }

        #region Private Helper Methods

        /// <summary>
        /// Parses a comma-separated skills string into individual skills.
        /// Removes duplicates and empty entries.
        /// </summary>
        private List<string> ParseSkills(string skills)
        {
            return skills.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => s.Trim())
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .Distinct(StringComparer.OrdinalIgnoreCase) // Remove duplicates case-insensitively
                        .ToList();
        }

        /// <summary>
        /// Generates intelligent tags based on course name.
        /// Removes duplicates and ensures unique tags per course.
        /// </summary>
        private List<string> GenerateCourseRelatedTags(string courseName)
        {
            var tags = new List<string> { courseName }; // Always include full course name

            // Subject keyword mapping for intelligent tag generation
            var subjectKeywords = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                {"Psychology", new List<string> {"Psychology", "Social Science"}},
                {"Biology", new List<string> {"Biology", "Life Science", "Science"}},
                {"Chemistry", new List<string> {"Chemistry", "Physical Science", "Science"}},
                {"Physics", new List<string> {"Physics", "Physical Science", "Science"}},
                {"Calculus", new List<string> {"Calculus", "Mathematics", "Math"}},
                {"Algebra", new List<string> {"Algebra", "Mathematics", "Math"}},
                {"Statistics", new List<string> {"Statistics", "Mathematics", "Math"}},
                {"Computer", new List<string> {"Computer Science", "Programming", "Technology"}},
                {"Programming", new List<string> {"Programming", "Computer Science", "Technology"}},
                {"Java", new List<string> {"Java", "Programming", "Computer Science"}},
                {"Python", new List<string> {"Python", "Programming", "Computer Science"}},
                {"JavaScript", new List<string> {"JavaScript", "Programming", "Web Development"}},
                {"History", new List<string> {"History", "Social Science"}},
                {"English", new List<string> {"English", "Literature", "Writing"}},
                {"Art", new List<string> {"Art", "Creative Arts"}},
                {"Music", new List<string> {"Music", "Performing Arts"}},
                {"Economics", new List<string> {"Economics", "Business", "Social Science"}},
                {"Business", new List<string> {"Business", "Management"}},
                {"Philosophy", new List<string> {"Philosophy", "Critical Thinking"}}
            };

            // Parse course name and add intelligent tags
            var words = courseName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            foreach (var word in words)
            {
                // Add level indicators
                if (word.Equals("Introduction", StringComparison.OrdinalIgnoreCase) || 
                    word.Equals("Intro", StringComparison.OrdinalIgnoreCase))
                {
                    tags.Add("Beginner");
                }
                else if (word.Equals("Advanced", StringComparison.OrdinalIgnoreCase))
                {
                    tags.Add("Advanced");
                }
                else if (word.Equals("Intermediate", StringComparison.OrdinalIgnoreCase))
                {
                    tags.Add("Intermediate");
                }

                // Check for subject matches
                foreach (var mapping in subjectKeywords)
                {
                    if (word.Contains(mapping.Key, StringComparison.OrdinalIgnoreCase))
                    {
                        tags.AddRange(mapping.Value);
                    }
                }
            }

            // Return distinct tags (case-insensitive) to prevent duplicates
            return tags.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        }

        #endregion
    }
}