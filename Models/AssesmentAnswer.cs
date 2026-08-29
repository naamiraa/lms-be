namespace be.Models;

public class AssessmentAnswer
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid AssessmentAttemptId { get; set; }
    public AssessmentAttempt AssessmentAttempt { get; set; } = null!;

    public Guid AssessmentQuestionId { get; set; }
    public AssessmentQuestion AssessmentQuestion { get; set; } = null!;

    public Guid SelectedOptionId { get; set; }
    public QuestionOption SelectedOption { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}