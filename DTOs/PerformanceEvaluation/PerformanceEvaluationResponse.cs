namespace be.DTOs.PerformanceEvaluation;

public class PerformanceEvaluationResponse
{
    public Guid SimulationSessionId { get; set; }

    public string ScenarioName { get; set; } = string.Empty;

    public string DomainName { get; set; } = string.Empty;

    public double TheoryScore { get; set; }

    public double SimulatorScore { get; set; }

    public double OverallScore { get; set; }

    public string Result { get; set; } = string.Empty;

    public int TotalViolations { get; set; }

    public int? CompletionTimeSeconds { get; set; }

    public PerformanceBreakdownResponse Breakdown { get; set; }
        = new();

    public List<PerformanceEventResponse> Events { get; set; }
        = new();
}