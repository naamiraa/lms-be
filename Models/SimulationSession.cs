namespace be.Models;

public class SimulationSession
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid UserId { get; set; }

    public User User { get; set; } = null!;

    public Guid SimulationScenarioId { get; set; }

    public SimulationScenario SimulationScenario { get; set; } = null!;

    public Guid AssessmentAttemptId { get; set; }

    public AssessmentAttempt AssessmentAttempt { get; set; } = null!;

    public string Status { get; set; } = "READY";

    public DateTime? StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public SimulationResult? Result { get; set; }

    public ICollection<SimulationEvent> Events { get; set; }
        = new List<SimulationEvent>();
}