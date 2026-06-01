using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using WorkoutApp.API.Data;
using workoutapp_API.DTOs;
using WorkoutApp.API.Models;
using System.Security.Claims;

namespace workoutapp_API.controllers
{
    /// <summary>
    /// Manages workout entries.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/workouts")]
    public class WorkoutController : ControllerBase
    {
        private readonly WorkoutDbContext _context;
        private readonly IMemoryCache _memoryCache;

        public WorkoutController(WorkoutDbContext context, IMemoryCache memoryCache)
        {
            _context = context;
            _memoryCache = memoryCache;
        }

        /// <summary>
        /// Returns a paginated list of workouts for the authenticated user.
        /// </summary>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Items per page (default: 20)</param>
        /// <response code="200">Success</response>
        /// <response code="401">Unauthorized</response>
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<PagedResult<WorkoutDTO>>> GetWorkoutsAsync(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var query = _context.Workouts
                .Include(w => w.Exercise)
                .Where(w => w.UserId == userId)
                .AsQueryable();

            var total = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(w => new WorkoutDTO
                {
                    WorkoutId = w.WorkoutId,
                    UserId = w.UserId,
                    ExerciseId = w.ExerciseId,
                    ExerciseName = w.Exercise.ExerciseName,
                    PrimaryMuscle = w.Exercise.PrimaryMuscle
                })
                .ToListAsync();

            return Ok(new PagedResult<WorkoutDTO>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalCount = total,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize)
            });
        }

        /// <summary>
        /// Returns a single workout belonging to the authenticated user.
        /// </summary>
        /// <param name="id">Workout ID</param>
        /// <response code="200">Returns the workout</response>
        /// <response code="404">Not found</response>
        /// <response code="401">Unauthorized</response>
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<WorkoutDTO>> GetWorkoutAsync(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var cacheKey = $"workout_{userId}_{id}";

            if (!_memoryCache.TryGetValue(cacheKey, out WorkoutDTO? cached))
            {
                var workout = await _context.Workouts
                    .Include(w => w.Exercise)
                    .FirstOrDefaultAsync(w => w.WorkoutId == id && w.UserId == userId);

                if (workout == null)
                    return NotFound();

                cached = new WorkoutDTO
                {
                    WorkoutId = workout.WorkoutId,
                    UserId = workout.UserId,
                    ExerciseId = workout.ExerciseId,
                    ExerciseName = workout.Exercise.ExerciseName,
                    PrimaryMuscle = workout.Exercise.PrimaryMuscle
                };

                _memoryCache.Set(cacheKey, cached, TimeSpan.FromMinutes(10));
            }

            return Ok(cached);
        }


        /// <summary>
        /// Creates a new workout entry for the authenticated user.
        /// </summary>
        /// <param name="dto">Workout details. The user ID is taken from the JWT token.</param>
        /// <response code="201">Created successfully</response>
        /// <response code="400">Invalid input</response>
        /// <response code="401">Unauthorized</response>
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<WorkoutDTO>> CreateWorkoutAsync([FromBody] CreateWorkoutDTO dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var exercise = await _context.Exercises
                .FirstOrDefaultAsync(e => e.ExerciseId == dto.ExerciseId);

            if (exercise == null)
            {
                return BadRequest(new { Message = "Exercise does not exist." });
            }

            var workout = new Workout
            {
                UserId = userId,
                ExerciseId = dto.ExerciseId
            };

            _context.Workouts.Add(workout);
            await _context.SaveChangesAsync();

            return Created($"/api/v1/workouts/{workout.WorkoutId}", new WorkoutDTO
            {
                WorkoutId = workout.WorkoutId,
                UserId = workout.UserId,
                ExerciseId = workout.ExerciseId,
                ExerciseName = exercise.ExerciseName,
                PrimaryMuscle = exercise.PrimaryMuscle

            });
        }

        /// <summary>
        /// Deletes a workout belonging to the authenticated user.
        /// </summary>
        /// <param name="id">Workout ID</param>
        /// <response code="204">Deleted successfully</response>
        /// <response code="404">Not found</response>
        /// <response code="401">Unauthorized</response>
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWorkoutAsync(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var workout = await _context.Workouts
                .FirstOrDefaultAsync(w => w.WorkoutId == id && w.UserId == userId);

            if (workout == null)
            {
                return NotFound();
            }

            _context.Workouts.Remove(workout);
            await _context.SaveChangesAsync();

            _memoryCache.Remove($"workout_{userId}_{id}");

            return NoContent();
        }
    }
}
