namespace be.DTOs.ParticipantAssessment;

public class StartAssessmentResponse
{
    public Guid AttemptId { get; set; }

    public Guid AssessmentId { get; set; }

    public string AssessmentTitle { get; set; } = string.Empty;

    public int AttemptNumber { get; set; }

    public int MaxAttempts { get; set; }

    public int DurationMinutes { get; set; }

    public DateTime StartedAt { get; set; }

    public DateTime ExpiresAt { get; set; }

    public string Status { get; set; } = string.Empty;

    public List<ParticipantQuestionResponse> Questions { get; set; } = new();
}