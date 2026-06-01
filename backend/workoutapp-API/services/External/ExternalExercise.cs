using Microsoft.Extensions.Caching.Memory;
using workoutapp_API.DTOs;
using workoutapp_API.services.External;

public class ExternalExercise : IExternalExercise
{
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _memoryCache;

    public ExternalExercise(HttpClient httpClient, IMemoryCache memoryCache)
    {
        _httpClient = httpClient;
        _memoryCache = memoryCache;
    }


    public async Task<IEnumerable<ExternalExerciseDTO>> GetExercisesAsync()
    {
        if (_memoryCache.TryGetValue("exercises", out List<ExternalExerciseDTO>? cachedExercises))
        {
            return cachedExercises!;
        }

        var response = await _httpClient.GetAsync("dist/exercises.json");
        response.EnsureSuccessStatusCode();

        var exercises = await response.Content.ReadFromJsonAsync<List<ExternalExerciseDTO>>()
                        ?? new List<ExternalExerciseDTO>();

        _memoryCache.Set("exercises", exercises, TimeSpan.FromMinutes(30));

        return exercises;
    }

    public async Task<ExternalExerciseDTO?> GetExerciseByIdAsync(string exerciseId)
    {
        var exercises = await GetExercisesAsync();
        return exercises.FirstOrDefault(e => e.Id == exerciseId);
    }
}