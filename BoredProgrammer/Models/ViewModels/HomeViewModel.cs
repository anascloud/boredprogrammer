using BoredProgrammer.Services;
using SyncSyntax.Models;

namespace BoredProgrammer.Models.ViewModels;

public class HomeViewModel
{
    public int PostCount { get; set; }
    public int CategoryCount { get; set; }
    public int CommentCount { get; set; }
    public int AiResponses { get; set; }
    public IReadOnlyList<Post> LatestPosts { get; set; } = Array.Empty<Post>();
    public IReadOnlyList<Category> Categories { get; set; } = Array.Empty<Category>();
}

public class DashboardViewModel
{
    public int PostCount { get; set; }
    public int CategoryCount { get; set; }
    public int CommentCount { get; set; }
    public int AiTotal { get; set; }
    public bool AiConfigured { get; set; }
    public AiUsageSnapshot AiUsage { get; set; } = new(0, 0, 0, null);
    public IReadOnlyList<Post> RecentPosts { get; set; } = Array.Empty<Post>();
    public IReadOnlyList<Comment> RecentComments { get; set; } = Array.Empty<Comment>();
    public IReadOnlyList<CategoryStat> CategoryStats { get; set; } = Array.Empty<CategoryStat>();
    public IReadOnlyList<MonthStat> MonthlyStats { get; set; } = Array.Empty<MonthStat>();
}

public record CategoryStat(string Name, int Count);
public record MonthStat(string Label, int Count);

public class ChatRequest
{
    public string Message { get; set; } = string.Empty;
    public List<ChatTurn> History { get; set; } = new();
}

public class ChatTurn
{
    public string Role { get; set; } = "user";
    public string Content { get; set; } = string.Empty;
}

public class SummarizeRequest
{
    public int PostId { get; set; }
}

public class GeneratePostRequest
{
    public string Topic { get; set; } = string.Empty;
    public string Tone { get; set; } = "professional";
}
