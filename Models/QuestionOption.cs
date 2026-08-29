namespace be.Models;

public class QuestionOption
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid AssessmentQuestionId { get; set; }
    public AssessmentQuestion AssessmentQuestion { get; set; } = null!;

    public string OptionText { get; set; } = string.Empty;

    public bool IsCorrect { get; set; }

    public int Order { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}