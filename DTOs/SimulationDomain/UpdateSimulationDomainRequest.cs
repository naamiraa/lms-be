namespace be.DTOs.SimulationDomain;

public class UpdateSimulationDomainRequest
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }
}