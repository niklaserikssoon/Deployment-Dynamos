using WorkoutApp.API.Models;
using workoutapp_API.DTOs;

namespace workoutapp_API.services.Exercises
{
    public interface IExerciseService
    {
        Task<List<Exercise>> GetLocalExercisesAsync();
        Task<Exercise> CreateExerciseAsync(CreateExerciseDTO dto);
        Task<Exercise?> UpdateExerciseAsync(int id, UpdateExerciseDTO dto);
        Task<bool> DeleteExerciseAsync(int id);
    }
}

