namespace be.DTOs.ParticipantAssessment;

public class SubmitAssessmentResponse
{
    public Guid AttemptId { get; set; }

    public string Status { get; set; } = string.Empty;

    public bool AssessmentCompleted { get; set; }

    public bool SimulationUnlocked { get; set; }

    public string Message { get; set; } = string.Empty;
}