namespace be.DTOs.InstructorFeedback;

public class ViolationExplanationRequest
{
    public Guid SimulationEventId { get; set; }

    public string Explanation { get; set; } = string.Empty;
}