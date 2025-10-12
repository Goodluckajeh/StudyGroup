using StudyGroup.Service.Interfaces;
using StudyGroup.Service.Settings;
using StudyGroup.Service.DTOs;
using StudyGroup.Data.Interfaces;
using StudyGroup.Data.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace StudyGroup.Service.Services
{
    /// <summary>
    /// Authentication service for user login and registration.
    /// Handles JWT token generation and user authentication using BCrypt.
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly JwtSettings _jwtSettings;

        /// <summary>
        /// Initializes a new instance of the AuthService.
        /// </summary>
        /// <param name="userRepo">The user repository for data access</param>
        /// <param name="jwtOptions">JWT configuration settings</param>
        public AuthService(IUserRepository userRepo, IOptions<JwtSettings> jwtOptions)
        {
            _userRepo = userRepo;
            _jwtSettings = jwtOptions.Value;
        }

        /// <summary>
        /// Authenticates a user by email and password and generates JWT token.
        /// </summary>
        /// <param name="email">The user's email address</param>
        /// <param name="password">The user's password</param>
        /// <returns>JWT token if authentication successful, null otherwise</returns>
        public async Task<string?> AuthenticateAsync(string email, string password)
        {
            var user = await _userRepo.GetByEmailAsync(email);
            if (user == null || !VerifyPassword(password, user.PasswordHash))
                return null;

            // Generate JWT token with user claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim("firstName", user.FirstName),
                new Claim("lastName", user.LastName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            
            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(_jwtSettings.ExpirationHours),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Registers a new user with hashed password.
        /// </summary>
        /// <param name="model">The user registration data</param>
        /// <returns>True if registration successful, false otherwise</returns>
        public async Task<bool> RegisterAsync(CreateUserDto model)
        {
            try
            {
                // Check if user already exists
                var existingUser = await _userRepo.GetByEmailAsync(model.Email);
                if (existingUser != null)
                    return false;

                var user = new User
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    PasswordHash = HashPassword(model.Password),
                    Skills = model.Skills,
                    Visibility = model.Visibility,
                    Bio = model.Bio
                };

                var result = await _userRepo.CreateAsync(user);
                return result > 0;
            }
            catch
            {
                return false;
            }
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
    }
}