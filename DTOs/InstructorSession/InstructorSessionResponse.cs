namespace be.DTOs.InstructorSession;

public class InstructorSessionResponse
{
    public Guid SimulationSessionId { get; set; }

    public string ParticipantName { get; set; } = string.Empty;

    public string ScenarioName { get; set; } = string.Empty;

    public double TheoryScore { get; set; }

    public double SimulatorScore { get; set; }

    public double OverallScore { get; set; }

    public string Result { get; set; } = string.Empty;

    public DateTime? CompletedAt { get; set; }

    public bool HasInstructorFeedback { get; set; }
}