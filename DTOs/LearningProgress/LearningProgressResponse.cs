namespace be.DTOs.LearningProgress;

public class LearningProgressResponse
{
    public Guid LearningContentId { get; set; }

    public string LearningContentTitle { get; set; } = string.Empty;

    public bool IsCompleted { get; set; }

    public DateTime? CompletedAt { get; set; }
}