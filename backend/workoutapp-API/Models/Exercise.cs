using System.ComponentModel.DataAnnotations;

namespace WorkoutApp.API.Models;

public class Exercise
{
    [Key]
    public int ExerciseId { get; set; }

    [StringLength(50)]
    public string ExternalExerciseId { get; set; } = null!;

    [StringLength(150)]
    public string ExerciseName { get; set; } = null!;

    [StringLength(100)]
    public string PrimaryMuscle { get; set; } = null!;

    public virtual ICollection<Workout> Workouts { get; set; } = new List<Workout>();
}