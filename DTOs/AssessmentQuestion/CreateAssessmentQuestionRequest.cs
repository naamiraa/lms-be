namespace be.DTOs.AssessmentQuestion;

public class CreateAssessmentQuestionRequest
{
    public Guid AssessmentId { get; set; }

    public string QuestionText { get; set; } = string.Empty;

    public string? MediaUrl { get; set; }

    public int Order { get; set; }

    public List<QuestionOptionRequest> Options { get; set; } = new();
}