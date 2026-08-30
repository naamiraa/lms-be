namespace be.DTOs.SimulationScenario;

public class SimulationScenarioResponse
{
    public Guid Id { get; set; }

    public Guid SimulationDomainId { get; set; }

    public string SimulationDomainName { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? Difficulty { get; set; }

    public int EstimatedMinutes { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}