using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace BoredProgrammer.Services;

public sealed class GroqOptions
{
    public string ApiKey { get; set; } = string.Empty;
    public string Model { get; set; } = "llama-3.3-70b-versatile";
    public string Endpoint { get; set; } = "https://api.groq.com/openai/v1/chat/completions";
}

public sealed record ChatMessage(string Role, string Content);

public sealed class GroqException : Exception
{
    public GroqException(string message) : base(message) { }
}

public sealed class GroqService
{
    private readonly HttpClient _http;
    private readonly GroqOptions _options;
    private readonly ILogger<GroqService> _logger;
    private readonly string _apiKey;

    public GroqService(HttpClient http, IOptions<GroqOptions> options, ILogger<GroqService> logger)
    {
        _http = http;
        _options = options.Value;
        _logger = logger;
        _apiKey = !string.IsNullOrWhiteSpace(_options.ApiKey)
            ? _options.ApiKey
            : Environment.GetEnvironmentVariable("GROQ_API_KEY") ?? string.Empty;

        _http.Timeout = TimeSpan.FromSeconds(45);
    }

    public bool IsConfigured => !string.IsNullOrWhiteSpace(_apiKey);

    public async Task<string> ChatAsync(
        IEnumerable<ChatMessage> messages,
        double temperature = 0.4,
        int maxTokens = 1200,
        bool json = false,
        CancellationToken cancellationToken = default)
    {
        if (!IsConfigured)
        {
            throw new GroqException("The AI service is not configured yet. Add a Groq API key to enable it.");
        }

        var body = new Dictionary<string, object?>
        {
            ["model"] = _options.Model,
            ["messages"] = messages.Select(m => new { role = m.Role, content = m.Content }).ToArray(),
            ["temperature"] = temperature,
            ["max_tokens"] = maxTokens
        };

        if (json)
        {
            body["response_format"] = new { type = "json_object" };
        }

        using var request = new HttpRequestMessage(HttpMethod.Post, _options.Endpoint)
        {
            Content = JsonContent.Create(body)
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

        using var response = await _http.SendAsync(request, cancellationToken);
        var raw = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Groq request failed with {Status}: {Body}", response.StatusCode, raw);
            throw new GroqException("The AI service is temporarily unavailable. Please try again.");
        }

        try
        {
            using var doc = JsonDocument.Parse(raw);
            var content = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return string.IsNullOrWhiteSpace(content)
                ? "I could not generate a response right now. Please try again."
                : content.Trim();
        }
        catch (Exception ex) when (ex is JsonException or KeyNotFoundException or IndexOutOfRangeException)
        {
            _logger.LogError(ex, "Could not parse Groq response: {Body}", raw);
            throw new GroqException("The AI service returned an unexpected response.");
        }
    }
}

public sealed class AiUsageTracker
{
    private int _chats;
    private int _summaries;
    private int _generations;
    private long _lastUsedTicks;

    public void RecordChat() => Record(ref _chats);
    public void RecordSummary() => Record(ref _summaries);
    public void RecordGeneration() => Record(ref _generations);

    private void Record(ref int counter)
    {
        Interlocked.Increment(ref counter);
        Interlocked.Exchange(ref _lastUsedTicks, DateTime.UtcNow.Ticks);
    }

    public AiUsageSnapshot Snapshot()
    {
        var last = Interlocked.Read(ref _lastUsedTicks);
        return new AiUsageSnapshot(
            Chats: Volatile.Read(ref _chats),
            Summaries: Volatile.Read(ref _summaries),
            Generations: Volatile.Read(ref _generations),
            LastUsedUtc: last == 0 ? null : new DateTime(last, DateTimeKind.Utc));
    }
}

public sealed record AiUsageSnapshot(int Chats, int Summaries, int Generations, DateTime? LastUsedUtc)
{
    public int Total => Chats + Summaries + Generations;
}
