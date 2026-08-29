namespace be.DTOs.AssessmentQuestion;

public class AssessmentQuestionResponse
{
    public Guid Id { get; set; }

    public Guid AssessmentId { get; set; }

    public string QuestionText { get; set; } = string.Empty;

    public string? MediaUrl { get; set; }

    public int Order { get; set; }

    public bool IsActive { get; set; }

    public List<QuestionOptionResponse> Options { get; set; } = new();
}