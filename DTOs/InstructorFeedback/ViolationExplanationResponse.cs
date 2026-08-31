namespace be.DTOs.InstructorFeedback;

public class ViolationExplanationResponse
{
    public Guid SimulationEventId { get; set; }

    public string EventType { get; set; } = string.Empty;

    public string? Severity { get; set; }

    public string Explanation { get; set; } = string.Empty;
}