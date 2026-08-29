namespace be.Models;

public class SimulationResult
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid SimulationSessionId { get; set; }

    public SimulationSession SimulationSession { get; set; } = null!;

    public double OverallSimulatorScore { get; set; }

    public double? SafetyScore { get; set; }

    public double? VehicleControlScore { get; set; }

    public double? EmergencyResponseScore { get; set; }

    public int? CompletionTimeSeconds { get; set; }

    public int TotalViolations { get; set; }

    public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}