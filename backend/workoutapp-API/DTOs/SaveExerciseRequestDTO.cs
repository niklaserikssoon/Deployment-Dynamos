using System.ComponentModel.DataAnnotations;

namespace workoutapp_API.DTOs
{
    public class SaveExerciseRequestDTO
    {
        public string? ExerciseId { get; set; }
        public string? Name { get; set; }
    }
}
