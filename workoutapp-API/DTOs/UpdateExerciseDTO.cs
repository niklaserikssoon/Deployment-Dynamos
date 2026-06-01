using System.ComponentModel.DataAnnotations;

namespace workoutapp_API.DTOs
{
    public class UpdateExerciseDTO
    {
        [Required(ErrorMessage = "Exercise name is required")]
        public string? ExerciseName { get; set; }

        [Required(ErrorMessage = "Primary muscle is required")]
        public string ? PrimaryMuscle { get; set; } 
    }
}
