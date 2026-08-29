namespace be.DTOs.LearningContent;

public class LearningContentResponse
{
    public Guid Id { get; set; }

    public Guid LearningModuleId { get; set; }

    public string LearningModuleName { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string ContentType { get; set; } = string.Empty;

    public string? ContentUrl { get; set; }

    public string? Description { get; set; }

    public int Order { get; set; }

    public int EstimatedMinutes { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}