using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using workoutapp_API.DTOs;
using workoutapp_API.services.AI;

namespace workoutapp_API.controllers
{
    /// <summary>
    /// AI-driven features.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/ai")]
    public class AiController : ControllerBase
    {
        private readonly IAiPlanService _aiPlanService;

        public AiController(IAiPlanService aiPlanService)
        {
            _aiPlanService = aiPlanService;
        }

        /// <summary>
        /// Generates a personalized workout plan using AI.
        /// </summary>
        /// <param name="request">User goals, fitness level, days per week and available equipment.</param>
        /// <response code="200">Returns the generated plan</response>
        /// <response code="400">Invalid input</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="502">AI service returned an unexpected error</response>
        /// <response code="503">AI service timed out</response>
        [Authorize]
        [HttpPost("generate-plan")]
        public async Task<ActionResult<GeneratePlanResponseDTO>> GeneratePlanAsync([FromBody] GeneratePlanRequestDTO request)
        {
            try
            {
                var result = await _aiPlanService.GeneratePlanAsync(request);
                return Ok(result);
            }
            catch (OperationCanceledException)
            {
                return StatusCode(503, new { Message = "AI service timed out. Please try again." });
            }
            catch (Exception)
            {
                return StatusCode(502, new { Message = "AI service is currently unavailable. Please try again later." });
            }
        }
    }
}
