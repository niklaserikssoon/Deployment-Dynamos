namespace workoutapp_API.DTOs
{
    public class ExternalExerciseDTO
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Level { get; set; } = string.Empty;

        public List<string> PrimaryMuscles { get; set; } = new();
    }
}
