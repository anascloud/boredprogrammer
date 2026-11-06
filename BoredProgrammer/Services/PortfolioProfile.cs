namespace BoredProgrammer.Services;

public sealed record SkillGroup(string Name, string Icon, string[] Skills);

public sealed record Experience(
    string Role,
    string Company,
    string Period,
    string Location,
    string Summary,
    string[] Highlights,
    string[] Stack);

public sealed record Project(
    string Name,
    string Tagline,
    string Description,
    string[] Stack,
    string Impact,
    string Icon);

public static class PortfolioProfile
{
    public const string Name = "Anas";
    public const string Role = "Senior Full-Stack .NET & AI Engineer";
    public const string ShortRole = ".NET + AI Engineer";
    public const string Tagline = "I design and ship production-grade SaaS, cloud and AI products.";
    public const string Location = "Remote — Worldwide";
    public const string Email = "anasbinsabiet@gmail.com";
    public const string GitHub = "https://github.com/anascloud";
    public const string LinkedIn = "https://linkedin.com/in/anasbd";
    public const int YearsExperience = 9;

    public const string Summary =
        "Senior full-stack engineer with 9 years of experience building secure, scalable web platforms and " +
        "AI-powered products. I work end-to-end — architecture, .NET APIs, modern React frontends, cloud " +
        "deployment and LLM integration — and I care deeply about clean code, performance and measurable " +
        "business outcomes.";

    public static readonly string[] Stats =
    {
        "9+ years shipping software",
        "40+ production releases",
        "8 enterprise clients",
        "4 AI products launched"
    };

    public static readonly SkillGroup[] Skills =
    {
        new("Backend & APIs", "bi-server",
            new[] { "C#", ".NET 10", "ASP.NET Core", "EF Core", "REST", "gRPC", "SignalR", "Hangfire" }),
        new("Frontend", "bi-window",
            new[] { "React", "Next.js", "TypeScript", "Tailwind CSS", "Blazor", "Razor", "Bootstrap" }),
        new("AI & LLM", "bi-stars",
            new[] { "Groq", "OpenAI", "Semantic Kernel", "LangChain", "RAG", "Vector search", "Agents", "Prompt engineering" }),
        new("Cloud & DevOps", "bi-cloud",
            new[] { "Azure", "AWS", "Docker", "Kubernetes", "GitHub Actions", "CI/CD", "Application Insights" }),
        new("Data & Messaging", "bi-database",
            new[] { "SQL Server", "PostgreSQL", "Redis", "MongoDB", "Qdrant", "RabbitMQ", "Kafka" }),
        new("Architecture", "bi-diagram-3",
            new[] { "Clean Architecture", "DDD", "CQRS", "Microservices", "Event-driven", "Caching", "Observability" })
    };

    public static readonly Experience[] Experience =
    {
        new("Senior Full-Stack & AI Engineer", "Independent / Consulting", "2022 — Present", "Remote",
            "Partner with startups and enterprises to design and deliver AI-enabled SaaS platforms.",
            new[]
            {
                "Architected a multi-tenant document intelligence platform processing 1.2M+ pages/month.",
                "Built RAG pipelines over Azure AI Search and private LLM gateways with sub-second retrieval.",
                "Cut cloud spend 38% by reworking caching, indexing and deployment topology."
            },
            new[] { ".NET", "React", "Azure", "OpenAI", "RAG", "Kubernetes" }),

        new("Lead Software Engineer", "FinTech Scale-up", "2020 — 2022", "Hybrid",
            "Led a squad of 6 engineers modernising a legacy payments platform.",
            new[]
            {
                "Migrated monolith modules to .NET microservices with zero-downtime releases.",
                "Introduced event-driven settlement flows handling 3M+ transactions per day.",
                "Reduced p95 API latency from 820ms to 190ms through profiling and caching."
            },
            new[] { ".NET Core", "SQL Server", "RabbitMQ", "Redis", "Docker" }),

        new("Senior Software Engineer", "Enterprise SaaS", "2018 — 2020", "On-site",
            "Owned the customer-facing portal used by 120k monthly active users.",
            new[]
            {
                "Rebuilt the frontend in React + TypeScript, lifting conversion by 24%.",
                "Shipped SSO (SAML/OIDC), RBAC and audit logging for enterprise customers.",
                "Mentored 5 junior developers and introduced code review + testing standards."
            },
            new[] { "C#", "ASP.NET MVC", "React", "TypeScript", "Azure AD" }),

        new("Software Engineer", "Software House", "2017 — 2018", "On-site",
            "Delivered bespoke web applications for clients across logistics and retail.",
            new[]
            {
                "Built inventory and reporting modules used daily by 2,000+ warehouse staff.",
                "Automated deployment with CI/CD pipelines, cutting release time by 70%."
            },
            new[] { "C#", "SQL Server", "JavaScript", "jQuery", "Azure" })
    };

    public static readonly Project[] Projects =
    {
        new("anascloud", "AI-powered content platform",
            "This live demo: ASP.NET Core MVC blog with Identity, AI assistant, AI post generation, streaming-ready UI and analytics.",
            new[] { ".NET 10", "Groq", "EF Core", "SQL Server", "Bootstrap" },
            "Live on runasp.net", "bi-braces"),
        new("DocuMind", "Document intelligence with RAG",
            "Multi-tenant document Q&A platform with OCR ingestion, vector search, citation-aware answers and audit trails.",
            new[] { "ASP.NET Core", "Semantic Kernel", "Qdrant", "React", "Azure" },
            "1.2M+ pages/month", "bi-file-earmark-text"),
        new("PulsePay", "Event-driven payments",
            "Composable payments platform with ledger, settlement, fraud scoring and real-time operational dashboards.",
            new[] { ".NET", "Kafka", "PostgreSQL", "Redis", "Kubernetes" },
            "3M+ daily transactions", "bi-credit-card")
    };

    public static readonly string[] Certifications =
    {
        "Microsoft Certified: Azure Solutions Architect Expert",
        "Microsoft Certified: Azure AI Engineer Associate",
        "AWS Certified Solutions Architect — Associate",
        "Certified Kubernetes Application Developer (CKAD)"
    };

    public static string ToPrompt() => $"""
        PROFILE
        Name: {Name}
        Role: {Role}
        Experience: {YearsExperience}+ years
        Location: {Location}
        Summary: {Summary}

        SKILLS
        {string.Join("\n", Skills.Select(s => $"- {s.Name}: {string.Join(", ", s.Skills)}"))}

        EXPERIENCE
        {string.Join("\n\n", Experience.Select(e => $"""
            {e.Role} @ {e.Company} ({e.Period}, {e.Location})
            {e.Summary}
            {string.Join("\n", e.Highlights.Select(h => "- " + h))}
            Stack: {string.Join(", ", e.Stack)}
            """))}

        PROJECTS
        {string.Join("\n\n", Projects.Select(p => $"""
            {p.Name} — {p.Tagline}
            {p.Description}
            Stack: {string.Join(", ", p.Stack)}
            Impact: {p.Impact}
            """))}

        CERTIFICATIONS
        {string.Join("\n", Certifications.Select(c => "- " + c))}
        """;
}
