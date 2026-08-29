namespace be.DTOs.AssessmentQuestion;

public class QuestionOptionResponse
{
    public Guid Id { get; set; }

    public string OptionText { get; set; } = string.Empty;

    public bool IsCorrect { get; set; }

    public int Order { get; set; }
}