using workoutapp_API.DTOs;

namespace workoutapp_API.services.AI
{
    public interface IAiPlanService
    {
        Task<GeneratePlanResponseDTO> GeneratePlanAsync(GeneratePlanRequestDTO request);
    }
}
