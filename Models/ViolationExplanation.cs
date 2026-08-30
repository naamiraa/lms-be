namespace be.Models;

public class ViolationExplanation
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid InstructorFeedbackId { get; set; }

    public InstructorFeedback InstructorFeedback { get; set; } = null!;

    public Guid SimulationEventId { get; set; }

    public SimulationEvent SimulationEvent { get; set; } = null!;

    public string Explanation { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}