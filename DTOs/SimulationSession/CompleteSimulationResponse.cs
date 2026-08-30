namespace be.DTOs.SimulationSession;

public class CompleteSimulationResponse
{
    public Guid SessionId { get; set; }

    public string Status { get; set; } = string.Empty;

    public double OverallSimulatorScore { get; set; }

    public int TotalViolations { get; set; }

    public DateTime CompletedAt { get; set; }

    public string Message { get; set; } = string.Empty;
}