using StudyGroup.Service.DTOs;

namespace StudyGroup.Service.Interfaces
{
    /// <summary>
    /// Interface for authentication services.
    /// Handles user authentication and JWT token generation.
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Authenticates a user by email and password.
        /// </summary>
        /// <param name="email">User's email address</param>
        /// <param name="password">User's password</param>
        /// <returns>JWT token if authentication successful, null otherwise</returns>
        Task<string?> AuthenticateAsync(string email, string password);

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="model">User registration data</param>
        /// <returns>True if registration successful, false otherwise</returns>
        Task<bool> RegisterAsync(CreateUserDto model);
    }
}