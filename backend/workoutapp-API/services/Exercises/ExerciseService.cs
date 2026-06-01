using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using WorkoutApp.API.Data;
using WorkoutApp.API.Models;
using workoutapp_API.DTOs;

namespace workoutapp_API.services.Exercises
{
    public class ExerciseService : IExerciseService
    {
        private readonly WorkoutDbContext _context;
        private readonly IMemoryCache _memoryCache;

        public ExerciseService(WorkoutDbContext context, IMemoryCache memoryCache)
        {
            _context = context;
            _memoryCache = memoryCache;
        }

        
        public async Task<List<Exercise>> GetLocalExercisesAsync()
        {
            if (_memoryCache.TryGetValue("local_exercises", out List<Exercise>? cachedExercises))
                return cachedExercises!;

            var exercises = await _context.Exercises.ToListAsync();

            _memoryCache.Set("local_exercises", exercises, TimeSpan.FromMinutes(30));

            return exercises;
        }

        public async Task<Exercise> CreateExerciseAsync(CreateExerciseDTO dto)
        {
            var exercise = new Exercise
            {
                ExerciseName = dto.ExerciseName,
                PrimaryMuscle = dto.PrimaryMuscle
            };

            _context.Exercises.Add(exercise);
            await _context.SaveChangesAsync();

            // Clear The Cache so next GET returns updated data
            _memoryCache.Remove("local_exercises");

            return exercise;
        }

        public async Task<Exercise?> UpdateExerciseAsync(int id, UpdateExerciseDTO dto)
        {
            var exercise = await _context.Exercises.FindAsync(id);

            if (exercise == null)
                return null;

            exercise.ExerciseName = dto.ExerciseName!;
            exercise.PrimaryMuscle = dto.PrimaryMuscle!;

            await _context.SaveChangesAsync();

            _memoryCache.Remove("local_exercises");

            return exercise;
        }


        public async Task<bool> DeleteExerciseAsync(int id)
        {
            var exercise = await _context.Exercises.FindAsync(id);

            if (exercise == null)
                return false;

            _context.Exercises.Remove(exercise);
            await _context.SaveChangesAsync();

            _memoryCache.Remove("local_exercises");

            return true;
        }



    }
}


