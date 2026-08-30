namespace be.DTOs.SimulationDomain;

public class CreateSimulationDomainRequest
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }
}