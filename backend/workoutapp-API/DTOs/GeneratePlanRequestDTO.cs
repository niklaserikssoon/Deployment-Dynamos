using System.ComponentModel.DataAnnotations;

namespace workoutapp_API.DTOs
{
    public class GeneratePlanRequestDTO
    {
        [Required]
        [StringLength(200)]
        public string Goal { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string FitnessLevel { get; set; } = string.Empty;

        [Range(1, 7)]
        public int DaysPerWeek { get; set; }

        [StringLength(100)]
        public string? Equipment { get; set; }
    }
}
