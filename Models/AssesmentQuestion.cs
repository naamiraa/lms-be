namespace be.Models;

public class AssessmentQuestion
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid AssessmentId { get; set; }
    public Assessment Assessment { get; set; } = null!;

    public string QuestionText { get; set; } = string.Empty;

    public string? MediaUrl { get; set; }

    public int Order { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<QuestionOption> Options { get; set; }
        = new List<QuestionOption>();
}