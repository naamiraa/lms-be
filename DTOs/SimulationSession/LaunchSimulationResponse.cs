namespace be.DTOs.SimulationSession;

public class LaunchSimulationResponse
{
    public Guid SessionId { get; set; }

    public string Status { get; set; } = string.Empty;

    public string ScenarioName { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;
}