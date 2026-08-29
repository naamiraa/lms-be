namespace be.DTOs.AssessmentQuestion;

public class UpdateAssessmentQuestionRequest
{
    public string QuestionText { get; set; } = string.Empty;

    public string? MediaUrl { get; set; }

    public int Order { get; set; }

    public List<QuestionOptionRequest> Options { get; set; } = new();
}