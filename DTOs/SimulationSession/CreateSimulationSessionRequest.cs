namespace be.DTOs.SimulationSession;

public class CreateSimulationSessionRequest
{
    public Guid AssessmentAttemptId { get; set; }

    public Guid SimulationScenarioId { get; set; }
}