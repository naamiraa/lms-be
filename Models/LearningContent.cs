namespace be.Models;

public class LearningContent
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid LearningModuleId { get; set; }
    public LearningModule LearningModule { get; set; } = null!;

    public string Title { get; set; } = string.Empty;

    public string ContentType { get; set; } = "TEXT";

    public string? ContentUrl { get; set; }

    public string? Description { get; set; }

    public int Order { get; set; }

    public int EstimatedMinutes { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}