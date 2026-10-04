namespace be.DTOs.ParticipantAssessment;

public class ParticipantQuestionResponse
{
    public Guid Id { get; set; }

    public string QuestionText { get; set; } = string.Empty;

    public string? MediaUrl { get; set; }

    public int Order { get; set; }

    public Guid? SelectedOptionId { get; set; }

    public List<ParticipantOptionResponse> Options { get; set; } = new();
}