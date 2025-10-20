using StudyGroup.Service.DTOs;

namespace StudyGroup.Service.Validation
{
    /// <summary>
    /// Validator for study group-related operations.
    /// Contains all business logic validation rules for study groups.
    /// </summary>
    public class StudyGroupValidator
    {
        /// <summary>
        /// Validates study group creation data.
        /// </summary>
        /// <param name="groupDto">The study group data to validate</param>
        /// <returns>Validation result with errors if any</returns>
        public ValidationResult ValidateCreate(CreateStudyGroupDto groupDto)
        {
            var result = new ValidationResult();

            // Required field validation
            if (string.IsNullOrWhiteSpace(groupDto.CourseName))
                result.AddError("CourseName", "Course name is required");

            if (groupDto.CreatorId <= 0)
                result.AddError("CreatorId", "Valid creator ID is required");

            // Course name validation
            if (!string.IsNullOrWhiteSpace(groupDto.CourseName))
            {
                if (groupDto.CourseName.Length > 100)
                    result.AddError("CourseName", "Course name cannot exceed 100 characters");

                if (groupDto.CourseName.Length < 3)
                    result.AddError("CourseName", "Course name must be at least 3 characters");
            }

            // Topic validation (optional but validate if provided)
            if (!string.IsNullOrWhiteSpace(groupDto.Topic))
            {
                if (groupDto.Topic.Length > 200)
                    result.AddError("Topic", "Topic cannot exceed 200 characters");
            }

            // Time slot validation (optional but validate if provided)
            if (!string.IsNullOrWhiteSpace(groupDto.TimeSlot))
            {
                if (groupDto.TimeSlot.Length > 100)
                    result.AddError("TimeSlot", "Time slot cannot exceed 100 characters");

                // Basic time slot format validation
                if (!IsValidTimeSlot(groupDto.TimeSlot))
                    result.AddError("TimeSlot", "Time slot format is invalid. Example: 'Mondays 6-8 PM' or 'Tuesdays 2:00-4:00 PM'");
            }

            // Description validation (optional but validate if provided)
            if (!string.IsNullOrWhiteSpace(groupDto.Description))
            {
                if (groupDto.Description.Length > 500)
                    result.AddError("Description", "Description cannot exceed 500 characters");
            }

            return result;
        }

        /// <summary>
        /// Validates study group update data.
        /// </summary>
        /// <param name="groupDto">The study group data to validate</param>
        /// <returns>Validation result with errors if any</returns>
        public ValidationResult ValidateUpdate(UpdateStudyGroupDto groupDto)
        {
            var result = new ValidationResult();

            // Required field validation
            if (string.IsNullOrWhiteSpace(groupDto.CourseName))
                result.AddError("CourseName", "Course name is required");

            // Course name validation
            if (!string.IsNullOrWhiteSpace(groupDto.CourseName))
            {
                if (groupDto.CourseName.Length > 100)
                    result.AddError("CourseName", "Course name cannot exceed 100 characters");

                if (groupDto.CourseName.Length < 3)
                    result.AddError("CourseName", "Course name must be at least 3 characters");
            }

            // Topic validation (optional but validate if provided)
            if (!string.IsNullOrWhiteSpace(groupDto.Topic))
            {
                if (groupDto.Topic.Length > 200)
                    result.AddError("Topic", "Topic cannot exceed 200 characters");
            }

            // Time slot validation (optional but validate if provided)
            if (!string.IsNullOrWhiteSpace(groupDto.TimeSlot))
            {
                if (groupDto.TimeSlot.Length > 100)
                    result.AddError("TimeSlot", "Time slot cannot exceed 100 characters");

                // Basic time slot format validation
                if (!IsValidTimeSlot(groupDto.TimeSlot))
                    result.AddError("TimeSlot", "Time slot format is invalid. Example: 'Mondays 6-8 PM' or 'Tuesdays 2:00-4:00 PM'");
            }

            // Description validation (optional but validate if provided)
            if (!string.IsNullOrWhiteSpace(groupDto.Description))
            {
                if (groupDto.Description.Length > 500)
                    result.AddError("Description", "Description cannot exceed 500 characters");
            }

            return result;
        }

        /// <summary>
        /// Validates group member creation data.
        /// </summary>
        /// <param name="memberDto">The group member data to validate</param>
        /// <returns>Validation result with errors if any</returns>
        public ValidationResult ValidateGroupMember(CreateGroupMemberDto memberDto)
        {
            var result = new ValidationResult();

            // Required field validation
            if (memberDto.GroupId <= 0)
                result.AddError("GroupId", "Valid group ID is required");

            if (memberDto.UserId <= 0)
                result.AddError("UserId", "Valid user ID is required");

            if (memberDto.StatusId <= 0)
                result.AddError("StatusId", "Valid status ID is required");

            // Status ID validation (should be 1=Pending, 2=Approved, 3=Rejected)
            if (memberDto.StatusId > 0 && !IsValidStatusId(memberDto.StatusId))
                result.AddError("StatusId", "Status ID must be 1 (Pending), 2 (Approved), or 3 (Rejected)");

            // For instant joining, recommend status 2
            if (memberDto.StatusId == 1)
            {
                result.AddError("StatusId", "Consider using StatusId = 2 for instant joining. Pending status (1) may require manual approval.");
            }

            return result;
        }

        #region Private Helper Methods

        /// <summary>
        /// Validates time slot format.
        /// Accepts various common formats like "Mondays 6-8 PM", "Tuesdays 2:00-4:00 PM", etc.
        /// </summary>
        private bool IsValidTimeSlot(string timeSlot)
        {
            if (string.IsNullOrWhiteSpace(timeSlot)) return false;

            // Basic patterns we accept
            var commonPatterns = new[]
            {
                @"^(Monday|Tuesday|Wednesday|Thursday|Friday|Saturday|Sunday)s?\s+\d{1,2}[:\-]\d{0,2}\s*(AM|PM|am|pm)?[\s\-]+\d{1,2}[:\-]\d{0,2}\s*(AM|PM|am|pm)?$",
                @"^(Monday|Tuesday|Wednesday|Thursday|Friday|Saturday|Sunday)s?\s+\d{1,2}\s*[\-]\s*\d{1,2}\s*(AM|PM|am|pm)?$",
                @"^(Mon|Tue|Wed|Thu|Fri|Sat|Sun)\s+\d{1,2}[:\-]\d{0,2}\s*(AM|PM|am|pm)?[\s\-]+\d{1,2}[:\-]\d{0,2}\s*(AM|PM|am|pm)?$"
            };

            // Check if it matches any common pattern OR contains day + time indicators
            var containsDay = new[] { "monday", "tuesday", "wednesday", "thursday", "friday", "saturday", "sunday", "mon", "tue", "wed", "thu", "fri", "sat", "sun" }
                .Any(day => timeSlot.ToLowerInvariant().Contains(day));

            var containsTime = timeSlot.Any(char.IsDigit) && (timeSlot.ToLowerInvariant().Contains("am") || 
                              timeSlot.ToLowerInvariant().Contains("pm") || 
                              timeSlot.Contains(":") || 
                              timeSlot.Contains("-"));

            // If it contains both day and time indicators, consider it valid
            // This is more lenient than strict regex matching
            return containsDay && containsTime;
        }

        /// <summary>
        /// Validates membership status ID.
        /// </summary>
        private bool IsValidStatusId(int statusId)
        {
            // Valid status IDs: 1=Pending, 2=Approved, 3=Rejected
            return statusId >= 1 && statusId <= 3;
        }

        #endregion
    }
}