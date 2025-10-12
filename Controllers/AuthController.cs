using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyGroup.Service.Interfaces;
using StudyGroup.Service.DTOs;

namespace StudyGroup.Api.Controllers
{
    /// <summary>
    /// Authentication controller for user registration and login.
    /// Handles JWT token generation.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        /// <summary>
        /// Initializes a new instance of the AuthController.
        /// </summary>
        /// <param name="authService">The authentication service</param>
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// POST: api/auth/register
        /// Registers a new user.
        /// 
        /// Example request body:
        /// {
        ///   "firstName": "John",
        ///   "lastName": "Doe",
        ///   "email": "john.doe@example.com",
        ///   "password": "SecurePassword123!",
        ///   "skills": "C#, React",
        ///   "visibility": true,
        ///   "bio": "Software developer"
        /// }
        /// </summary>
        /// <param name="model">User registration data</param>
        /// <returns>Success message if registration successful</returns>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] CreateUserDto model)
        {
            var result = await _authService.RegisterAsync(model);
            if (!result)
            {
                return BadRequest(new { Error = "Registration failed. User may already exist." });
            }

            return Ok(new { Message = "User registered successfully" });
        }

        /// <summary>
        /// POST: api/auth/login
        /// Authenticates a user and returns a JWT token.
        /// 
        /// Example request body:
        /// {
        ///   "email": "john.doe@example.com",
        ///   "password": "SecurePassword123!"
        /// }
        /// </summary>
        /// <param name="model">Login credentials</param>
        /// <returns>JWT token if authentication successful</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var token = await _authService.AuthenticateAsync(model.Email, model.Password);
            if (token == null)
            {
                return Unauthorized(new { Error = "Invalid email or password" });
            }

            return Ok(new 
            { 
                Token = token,
                TokenType = "Bearer",
                ExpiresIn = "1 hour",
                Message = "Login successful"
            });
        }
    }

    /// <summary>
    /// DTO for user login.
    /// </summary>
    public class LoginDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}