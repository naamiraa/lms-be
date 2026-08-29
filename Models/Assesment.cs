namespace be.Models;

public class Assessment
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid LearningModuleId { get; set; }
    public LearningModule LearningModule { get; set; } = null!;

    public string Title { get; set; } = string.Empty;

    public string Type { get; set; } = "POST_TEST";

    public string? Description { get; set; }

    public int DurationMinutes { get; set; }

    public double PassingScore { get; set; }

    public int MaxAttempts { get; set; } = 1;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<AssessmentQuestion> Questions { get; set; }
        = new List<AssessmentQuestion>();

    public ICollection<AssessmentAttempt> Attempts { get; set; }
        = new List<AssessmentAttempt>();
}