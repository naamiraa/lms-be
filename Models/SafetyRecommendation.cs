namespace be.Models;

public class SafetyRecommendation
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid InstructorFeedbackId { get; set; }

    public InstructorFeedback InstructorFeedback { get; set; } = null!;

    public string Description { get; set; } = string.Empty;

    public int Priority { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}