using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using WorkoutApp.API.Data;
using WorkoutApp.API.Models;
using workoutapp_API.DTOs;
using workoutapp_API.services.Exercises;
using workoutapp_API.services.External;

namespace workoutapp_API.controllers
{
    /// <summary>
    /// Manages exercises.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/exercises")]
    public class ExerciseController : ControllerBase
    {
        private readonly IExternalExercise _externalExercise;
        private readonly IExerciseService _exerciseService;
        private readonly ISaveExerciseService _saveExerciseService;

        public ExerciseController(IExternalExercise externalExercise, IExerciseService exerciseService, ISaveExerciseService saveExerciseService)
        {
            _externalExercise = externalExercise;
            _exerciseService = exerciseService;
            _saveExerciseService = saveExerciseService;
        }


        /// <summary>
        /// Returns a paginated list of exercises.
        /// </summary>
        /// <param name="name">Optional name filter</param>
        /// <param name="level">Optional level filter (e.g. beginner, intermediate, expert)</param>
        /// <param name="muscle">Optional muscle group filter</param>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Items per page (default: 20)</param>
        /// <response code="200">Success</response>
        [HttpGet]
        [AllowAnonymous] // [AllowAnonymous] replaces <response code="401">Unauthorized</response>
        public async Task<ActionResult<PagedResult<ExternalExerciseDTO>>> GetExercises(
            [FromQuery] string? name,
            [FromQuery] string? level,
            [FromQuery] string? muscle,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var exercises = await _externalExercise.GetExercisesAsync();

            if (!string.IsNullOrWhiteSpace(name))
                exercises = exercises.Where(e => e.Name.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();

            if (!string.IsNullOrWhiteSpace(level))
                exercises = exercises.Where(e => e.Level.Equals(level, StringComparison.OrdinalIgnoreCase)).ToList();

            if (!string.IsNullOrWhiteSpace(muscle))
                exercises = exercises.Where(e => e.PrimaryMuscles.Any(m => m.Contains(muscle, StringComparison.OrdinalIgnoreCase))).ToList();

            var total = exercises.Count();
            var items = exercises.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return Ok(new PagedResult<ExternalExerciseDTO>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalCount = total,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize)
            });
        }

        /// <summary>
        /// Saves an exercise from the external API to the local database.
        /// </summary>
        /// <param name="id">External exercise ID.</param>
        /// <response code="200">Exercise saved successfully.</response>
        /// <response code="401">The user is not authenticated or the token is invalid.</response>
        /// <response code="404">Exercise not found in the external API.</response>
        [Authorize]
        [HttpPost("save/{id}")]
        public async Task<IActionResult> SaveExercise(string id)
        {
            var savedExercise = await _saveExerciseService.SaveExerciseAsync(id);
            if (savedExercise == null)
            {
                return NotFound(new { Message = $"Exercise with ID '{id}' not found in external API." });
            }
            return Ok(savedExercise);
        }


        /// <summary>
        /// Returns all exercises stored in the local SQL database.
        /// </summary>
        /// <response code="200">Exercises retrieved successfully</response>
        [HttpGet("local")]
        public async Task<IActionResult> GetLocalExercises()
        {
            var exercises = await _exerciseService.GetLocalExercisesAsync();
            return Ok(exercises);
        }

        /// <summary>
        /// Creates a new exercise and saves it to the local SQL database.
        /// </summary>
        /// <param name="dto">Exercise data to create</param>
        /// <response code="201">Exercise created successfully</response>
        /// <response code="400">Invalid input data</response>
        [HttpPost]
        public async Task<IActionResult> CreateExercise(CreateExerciseDTO dto)
        {
            var exercise = await _exerciseService.CreateExerciseAsync(dto);

            return CreatedAtAction(nameof(GetLocalExercises), new { id = exercise.ExerciseId }, exercise);
        }


        /// <summary>
        /// Updates an existing exercise in the local SQL database.
        /// </summary>
        /// <param name="id">Exercise id to update</param>
        /// <param name="dto">Updated exercise data</param>
        /// <response code="200">Exercise updated successfully</response>
        /// <response code="404">Exercise not found</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateExercise(int id, UpdateExerciseDTO dto)
        {
            var updatedExercise = await _exerciseService.UpdateExerciseAsync(id, dto);

            if (updatedExercise == null)
                return NotFound();

            return Ok(updatedExercise);
        }


        /// <summary>
        /// Deletes an exercise from the local SQL database.
        /// </summary>
        /// <param name="id">Exercise id to delete</param>
        /// <response code="204">Exercise deleted successfully</response>
        /// <response code="404">Exercise not found</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExercise(int id)
        {
            var deleted = await _exerciseService.DeleteExerciseAsync(id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}