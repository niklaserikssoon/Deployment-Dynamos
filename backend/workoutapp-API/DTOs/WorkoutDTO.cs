using System.ComponentModel.DataAnnotations;

namespace workoutapp_API.DTOs
{
    public class WorkoutDTO
    {
        public int WorkoutId { get; set; }
        public int UserId { get; set; }
        public int ExerciseId { get; set; }
        public string? ExerciseName { get; set; }
        public string? PrimaryMuscle { get; set; }
    }

    public class CreateWorkoutDTO
    {
        [Required]
        public int ExerciseId { get; set; }
    }
}