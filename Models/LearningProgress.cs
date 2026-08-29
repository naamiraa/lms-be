namespace be.Models;

public class LearningProgress
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid LearningContentId { get; set; }
    public LearningContent LearningContent { get; set; } = null!;

    public bool IsCompleted { get; set; } = false;

    public DateTime? CompletedAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}