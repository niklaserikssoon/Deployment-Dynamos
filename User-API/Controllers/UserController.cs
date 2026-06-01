using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using User_API.DTOs;
using User_API.Service;
using System.Security.Claims;

namespace User_API.Controllers
{
    /// <summary>
    /// Manages user registration and authentication.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]

    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;

        public UserController(IUserService userService, ITokenService tokenService)
        {
            _userService = userService;
            _tokenService = tokenService;
        }

        /// <summary>
        /// Registers a new user account.
        /// </summary>
        /// <param name="userCreateDto">User registration details</param>
        /// <response code="200">User created successfully</response>
        /// <response code="400">Invalid input</response>
        [HttpPost("register")]
        public async Task<IActionResult> CreateUserAsync([FromBody] CreateUserDTO userCreateDto)
        {
            var user = await _userService.CreateUserAsync(userCreateDto);

            return Ok(new UserResponseDTO
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName
            });
        }

        /// <summary>
        /// Authenticates a user with username and password.
        /// </summary>
        /// <param name="dto">Login credentials</param>
        /// <response code="200">Login successful, returns JWT token and user info.</response>
        /// <response code="400">Missing credentials</response>
        /// <response code="401">Invalid username or password</response>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.UserName) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest(new { message = "Username and password are required." });

            var user = await _userService.GetUserByUsernameAsync(dto.UserName);

            if (user == null)
                return Unauthorized(new { message = "Wrong username or password." });

            var passwordIsValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

            if (!passwordIsValid)
                return Unauthorized(new { message = "Wrong username or password." });

            var token = _tokenService.CreateToken(user);

            return Ok(new LoginResponseDTO
            {
                Token = token,
                ExpiresAtUtc = DateTime.UtcNow.AddHours(1),
                UserId = user.UserId,
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName
            });
        }

        /// <summary>
        /// Retrieves a user by ID.
        /// </summary>
        /// <param name="id">The user ID</param>
        /// <response code="200">Returns the user</response>
        /// <response code="404">User not found</response>
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserAsync(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            if (id != userId)
                return Forbid();

            var user = await _userService.GetUserByIdAsync(userId);

            if (user == null)
                return NotFound();

            return Ok(new UserResponseDTO
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName
            });
        }

        /// <summary>
        /// Updates an existing user's details.
        /// </summary>
        /// <param name="id">The user ID</param>
        /// <param name="dto">Updated user details</param>
        /// <response code="204">Update successful</response>
        /// <response code="400">Invalid input</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">Forbidden - user can only update their own account.</response>
        /// <response code="404">User not found</response>
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserAsync(int id, [FromBody] UpdateUserDTO dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            if (id != userId)
            {
                return Forbid();
            }

            var user = await _userService.UpdateUserAsync(userId, dto);

            if (user == null)
            {
                return NotFound();
            }

            return NoContent();
        }

        /// <summary>
        /// Deletes a user by ID.
        /// </summary>
        /// <param name="id">The user ID</param>
        /// <response code="204">Deletion successful</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">Forbidden - user can only delete their own account.</response>
        /// <response code="404">User not found</response>
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserAsync(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            if (id != userId)
            {
                return Forbid();
            }

            var deleted = await _userService.DeleteUserAsync(userId);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}