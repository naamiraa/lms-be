namespace be.DTOs.SimulationScenario;

public class CreateSimulationScenarioRequest
{
    public Guid SimulationDomainId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? Difficulty { get; set; }

    public int EstimatedMinutes { get; set; }
}