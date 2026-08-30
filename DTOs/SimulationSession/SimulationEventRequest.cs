namespace be.DTOs.SimulationSession;

public class SimulationEventRequest
{
    public string EventType { get; set; } = string.Empty;

    public string? Severity { get; set; }

    public string? Description { get; set; }

    public double? OccurredAtSeconds { get; set; }

    public double? PenaltyPoints { get; set; }
}