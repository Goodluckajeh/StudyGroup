using System.Collections.Generic;
using System.Threading.Tasks;
using StudyGroup.Data.Models;
using StudyGroup.Data.Interfaces;
using StudyGroup.Service.Interfaces;
using StudyGroup.Service.DTOs;
using StudyGroup.Service.Validation;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace StudyGroup.Service.Services
{
    /// <summary>
    /// Implements IUserService, contains business logic for users.
    /// Handles password hashing using SHA256 for compatibility with AuthService.
    /// Automatically manages skill tags when users update their skills.
    /// Includes comprehensive validation for all user operations.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly UserValidator _validator;

        /// <summary>
        /// Initializes a new instance of the UserService.
        /// </summary>
        /// <param name="repository">The user repository for data access</param>
        public UserService(IUserRepository repository)
        {
            _repository = repository;
            _validator = new UserValidator();
        }

        /// <summary>
        /// Hashes a password using SHA256.
        /// </summary>
        /// <param name="password">The plain text password</param>
        /// <returns>Hashed password</returns>
        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Verifies a password against its hash.
        /// </summary>
        /// <param name="password">The plain text password</param>
        /// <param name="hash">The stored password hash</param>
        /// <returns>True if password matches, false otherwise</returns>
        private bool VerifyPassword(string password, string hash)
        {
            return HashPassword(password) == hash;
        }

        /// <summary>
        /// Maps User model (PascalCase) to UserDto (PascalCase).
        /// Excludes sensitive password hash from the response.
        /// </summary>
        /// <param name="user">The user model from database</param>
        /// <returns>DTO representation without sensitive data</returns>
        private UserDto MapToDto(User user) => new UserDto
        {
            UserId = user.UserId,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Skills = user.Skills,
            Visibility = user.Visibility,
            Bio = user.Bio
        };

        /// <summary>
        /// Retrieves all users from the system.
        /// Password hashes are excluded from the response for security.
        /// </summary>
        /// <returns>All users without sensitive password data</returns>
        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _repository.GetAllAsync();
            return users.Select(MapToDto);
        }

        /// <summary>
        /// Gets a specific user by their ID.
        /// Password hash is excluded from the response for security.
        /// </summary>
        /// <param name="userId">The unique identifier of the user</param>
        /// <returns>The user without sensitive password data if found, null otherwise</returns>
        public async Task<UserDto?> GetUserByIdAsync(int userId)
        {
            var user = await _repository.GetByIdAsync(userId);
            return user == null ? null : MapToDto(user);
        }

        /// <summary>
        /// Creates a new user with a securely hashed password.
        /// Includes comprehensive validation before creation.
        /// The user account is immediately active and ready to use.
        /// Note: Skill tags are NOT automatically created here to avoid circular dependency.
        /// Use AutoTaggingService separately after user creation if needed.
        /// </summary>
        /// <param name="userDto">The user data including plain text password</param>
        /// <returns>The ID of the created user</returns>
        /// <exception cref="InvalidOperationException">Thrown when validation fails or email already exists</exception>
        public async Task<int> CreateUserAsync(CreateUserDto userDto)
        {
            // Step 1: Validate input data
            var validationResult = _validator.ValidateCreate(userDto);
            if (!validationResult.IsValid)
            {
                throw new InvalidOperationException($"Validation failed: {validationResult.GetErrorsAsString()}");
            }

            // Step 2: Check if email already exists
            var existingUser = await _repository.GetByEmailAsync(userDto.Email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("An account with this email address already exists");
            }

            // Step 3: Hash the password using SHA256
            var hashedPassword = HashPassword(userDto.Password);

            // Step 4: Create user entity
            var user = new User
            {
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Email = userDto.Email,
                PasswordHash = hashedPassword,
                Skills = userDto.Skills,
                Visibility = userDto.Visibility,
                Bio = userDto.Bio
            };
            
            var userId = await _repository.CreateAsync(user);
            
            // Return the user ID so calling code can handle skill tagging if needed
            return userId;
        }

        /// <summary>
        /// Updates an existing user's information.
        /// Includes comprehensive validation before update.
        /// Note: This method does not update passwords. Use ChangePasswordAsync for password changes.
        /// Note: Skill tags are NOT automatically updated here to avoid circular dependency.
        /// Use AutoTaggingService separately after user update if needed.
        /// </summary>
        /// <param name="userId">The ID of the user to update</param>
        /// <param name="userDto">The updated user data (excluding password)</param>
        /// <returns>True if updated successfully, false if user not found</returns>
        /// <exception cref="InvalidOperationException">Thrown when validation fails</exception>
        public async Task<bool> UpdateUserAsync(int userId, UpdateUserDto userDto)
        {
            // Step 1: Validate input data
            var validationResult = _validator.ValidateUpdate(userDto);
            if (!validationResult.IsValid)
            {
                throw new InvalidOperationException($"Validation failed: {validationResult.GetErrorsAsString()}");
            }

            // Step 2: Get existing user
            var user = await _repository.GetByIdAsync(userId);
            if (user == null) return false;
            
            // Step 3: Update user information (email and password hash remain unchanged for security)
            user.FirstName = userDto.FirstName;
            user.LastName = userDto.LastName;
            user.Skills = userDto.Skills;
            user.Visibility = userDto.Visibility;
            user.Bio = userDto.Bio;
            
            var result = await _repository.UpdateAsync(user);
            
            // Return success status so calling code can handle skill tagging if needed
            return result;
        }

        /// <summary>
        /// Deletes a user by their ID.
        /// This will also cascade delete related group memberships.
        /// Note: Associated skill tags should be cleaned up by the calling code.
        /// </summary>
        /// <param name="userId">The ID of the user to delete</param>
        /// <returns>True if deleted successfully, false if user not found</returns>
        public async Task<bool> DeleteUserAsync(int userId)
        {
            return await _repository.DeleteAsync(userId);
        }

        /// <summary>
        /// Verifies a user's password against the stored hash.
        /// This method can be used for authentication/login purposes.
        /// </summary>
        /// <param name="email">The user's email address</param>
        /// <param name="password">The plain text password to verify</param>
        /// <returns>The user DTO if password is correct, null otherwise</returns>
        public async Task<UserDto?> VerifyPasswordAsync(string email, string password)
        {
            var user = await _repository.GetByEmailAsync(email);
            if (user == null) return null;

            // Verify the password using SHA256
            var isPasswordValid = VerifyPassword(password, user.PasswordHash);
            
            return isPasswordValid ? MapToDto(user) : null;
        }

        /// <summary>
        /// Changes a user's password.
        /// Includes comprehensive validation before changing password.
        /// Verifies the current password before setting the new one.
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        /// <param name="currentPassword">The user's current password</param>
        /// <param name="newPassword">The new password to set</param>
        /// <returns>True if password changed successfully, false if current password is incorrect or user not found</returns>
        /// <exception cref="InvalidOperationException">Thrown when validation fails</exception>
        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            // Step 1: Create DTO for validation
            var changePasswordDto = new ChangePasswordDto
            {
                CurrentPassword = currentPassword,
                NewPassword = newPassword,
                ConfirmNewPassword = newPassword // Assume confirmation matches for this method
            };

            // Step 2: Validate password change data
            var validationResult = _validator.ValidatePasswordChange(changePasswordDto);
            if (!validationResult.IsValid)
            {
                throw new InvalidOperationException($"Password validation failed: {validationResult.GetErrorsAsString()}");
            }

            // Step 3: Get user and verify current password
            var user = await _repository.GetByIdAsync(userId);
            if (user == null) return false;

            // Verify current password
            var isCurrentPasswordValid = VerifyPassword(currentPassword, user.PasswordHash);
            if (!isCurrentPasswordValid) return false;

            // Step 4: Hash the new password and update
            user.PasswordHash = HashPassword(newPassword);
            return await _repository.UpdateAsync(user);
        }
    }
}
