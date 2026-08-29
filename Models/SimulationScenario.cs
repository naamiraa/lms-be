namespace be.Models;

public class SimulationScenario
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid SimulationDomainId { get; set; }

    public SimulationDomain SimulationDomain { get; set; } = null!;

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? Difficulty { get; set; }

    public int EstimatedMinutes { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<SimulationSession> Sessions { get; set; }
        = new List<SimulationSession>();
}