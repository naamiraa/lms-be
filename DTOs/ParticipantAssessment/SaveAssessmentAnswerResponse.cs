namespace be.DTOs.ParticipantAssessment;

public class SaveAssessmentAnswerResponse
{
    public Guid QuestionId { get; set; }

    public Guid SelectedOptionId { get; set; }

    public DateTime SavedAt { get; set; }
}