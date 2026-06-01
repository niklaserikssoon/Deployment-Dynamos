using OpenAI;
using OpenAI.Chat;
using System.ClientModel;
using workoutapp_API.DTOs;

namespace workoutapp_API.services.AI
{
    public class AiPlanService : IAiPlanService
    {
        private readonly ChatClient _chatClient;

        public AiPlanService(IConfiguration configuration)
        {
            var apiKey = configuration["Groq:ApiKey"]
                ?? throw new InvalidOperationException("Groq API key is missing.");

            var openAiClient = new OpenAIClient(
                new ApiKeyCredential(apiKey),
                new OpenAIClientOptions { Endpoint = new Uri("https://api.groq.com/openai/v1") }
            );

            _chatClient = openAiClient.GetChatClient("llama-3.3-70b-versatile");
        }

        public async Task<GeneratePlanResponseDTO> GeneratePlanAsync(GeneratePlanRequestDTO request)
        {
            var equipment = string.IsNullOrWhiteSpace(request.Equipment) ? "valfri utrustning" : request.Equipment;

            var prompt = $"""
                Du är en erfaren personlig tränare. Skapa ett träningsschema baserat på följande:
                - Mål: {request.Goal}
                - Nivå: {request.FitnessLevel}
                - Träningsdagar per vecka: {request.DaysPerWeek}
                - Utrustning: {equipment}

                Svara med ett tydligt veckoschema. För varje träningsdag, lista övningar med set och reps.
                Håll svaret koncist och strukturerat. Svara på svenska.
                """;

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

            var response = await _chatClient.CompleteChatAsync(
                [new UserChatMessage(prompt)],
                new ChatCompletionOptions { MaxOutputTokenCount = 600 },
                cts.Token
            );

            var plan = response.Value.Content[0].Text;

            return new GeneratePlanResponseDTO { Plan = plan };
        }
    }
}
