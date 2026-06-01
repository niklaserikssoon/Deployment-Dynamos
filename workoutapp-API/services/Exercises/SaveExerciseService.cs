using Microsoft.EntityFrameworkCore;
using WorkoutApp.API.Data;
using WorkoutApp.API.Models;
using workoutapp_API.services.Exercises;
using workoutapp_API.services.External;

namespace workoutapp_API.services.Exercises
{
    public class SaveExerciseService : ISaveExerciseService
    {
        private readonly WorkoutDbContext _context;
        private readonly IExternalExercise _externalExerciseService;

        public SaveExerciseService(
            WorkoutDbContext context,
            IExternalExercise externalExerciseService)
        {
            _context = context;
            _externalExerciseService = externalExerciseService;
        }

        public async Task<Exercise?> SaveExerciseAsync(string externalExerciseId)
        {
            var existingExercise = await _context.Exercises
                .FirstOrDefaultAsync(e => e.ExternalExerciseId == externalExerciseId);

            if (existingExercise != null)
            {
                return existingExercise;
            }

            var exercises = await _externalExerciseService.GetExercisesAsync();
            var externalExercise = exercises.FirstOrDefault(e => e.Id == externalExerciseId);

            if (externalExercise == null)
            {
                return null;
            }

            var newExercise = new Exercise
            {
                ExternalExerciseId = externalExercise.Id,
                ExerciseName = externalExercise.Name,
                PrimaryMuscle = externalExercise.PrimaryMuscles.FirstOrDefault() ?? "Unknown"
            };


            _context.Exercises.Add(newExercise);
            await _context.SaveChangesAsync();

            return newExercise;
        }
    }
}
