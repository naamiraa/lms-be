namespace be.DTOs.AssessmentQuestion;

public class QuestionOptionRequest
{
    public string OptionText { get; set; } = string.Empty;

    public bool IsCorrect { get; set; }

    public int Order { get; set; }
}