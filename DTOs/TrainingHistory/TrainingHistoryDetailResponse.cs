namespace be.DTOs.TrainingHistory;

public class TrainingHistoryDetailResponse
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

    public int TotalViolations { get; set; }

    public double? SafetyScore { get; set; }

    public double? VehicleControlScore { get; set; }

    public double? EmergencyResponseScore { get; set; }

    public double? CompletionTimeSeconds { get; set; }

    public bool HasInstructorFeedback { get; set; }
}
