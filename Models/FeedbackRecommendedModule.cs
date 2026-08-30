namespace be.Models;

public class FeedbackRecommendedModule
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid InstructorFeedbackId { get; set; }

    public InstructorFeedback InstructorFeedback { get; set; } = null!;

    public Guid LearningModuleId { get; set; }

    public LearningModule LearningModule { get; set; } = null!;

    public string? RecommendationType { get; set; }

    public int Order { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}