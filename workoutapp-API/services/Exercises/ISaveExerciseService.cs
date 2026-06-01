using WorkoutApp.API.Models;

namespace workoutapp_API.services.Exercises
{
    public interface ISaveExerciseService
    {
        Task<Exercise?> SaveExerciseAsync(string externalExerciseId);
    }
}