using workoutapp_API.DTOs;

namespace workoutapp_API.services.External
{
    public interface IExternalExercise
    {
        Task<IEnumerable<ExternalExerciseDTO>> GetExercisesAsync();
        Task<ExternalExerciseDTO?> GetExerciseByIdAsync(string exerciseId);
    }
}
