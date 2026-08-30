namespace be.DTOs.SimulationSession;

public class StartSimulationResponse
{
    public Guid SessionId { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime StartedAt { get; set; }

    public string Message { get; set; } = string.Empty;
}