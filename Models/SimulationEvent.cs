namespace be.Models;

public class SimulationEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid SimulationSessionId { get; set; }

    public SimulationSession SimulationSession { get; set; } = null!;

    public string EventType { get; set; } = string.Empty;

    public string? Severity { get; set; }

    public string? Description { get; set; }

    public double? OccurredAtSeconds { get; set; }

    public double? PenaltyPoints { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}