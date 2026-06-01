using System.ComponentModel.DataAnnotations;

namespace workoutapp_API.DTOs
{
    public class CreateExerciseDTO
    {
        [Required(ErrorMessage = "Exercise name is required")]
        [StringLength(100, ErrorMessage = "Max 100 characters")]
        public string ExerciseName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Primary muscle is required")]
        [StringLength(100, ErrorMessage = "Max 100 characters")]
        public string PrimaryMuscle { get; set; } = string.Empty;
    }
}
