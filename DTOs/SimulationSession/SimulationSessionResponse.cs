namespace be.DTOs.SimulationSession;

public class SimulationSessionResponse
{
    public Guid Id { get; set; }

    public Guid AssessmentAttemptId { get; set; }

    public Guid SimulationScenarioId { get; set; }

    public string SimulationScenarioName { get; set; } = string.Empty;

    public string SimulationDomainName { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public DateTime? StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public DateTime CreatedAt { get; set; }
}