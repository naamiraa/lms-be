namespace be.DTOs.LearningContent;

public class CreateLearningContentRequest
{
    public Guid LearningModuleId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string ContentType { get; set; } = string.Empty;

    public string? ContentUrl { get; set; }

    public string? Description { get; set; }

    public int Order { get; set; }

    public int EstimatedMinutes { get; set; }
}