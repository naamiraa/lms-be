namespace be.DTOs.TrainingHistory;

public class TrainingHistoryItemResponse
{
    public Guid AssessmentAttemptId { get; set; }

    public Guid SimulationSessionId { get; set; }

    public string TrainingDomainName { get; set; } = string.Empty;

    public string LearningModuleName { get; set; } = string.Empty;

    public string AssessmentTitle { get; set; } = string.Empty;

    public string ScenarioName { get; set; } = string.Empty;

    public double TheoryScore { get; set; }

    public double SimulatorScore { get; set; }

    public double OverallScore { get; set; }

    public string Result { get; set; } = string.Empty;

    public DateTime CompletedAt { get; set; }

    public bool HasInstructorFeedback { get; set; }
}