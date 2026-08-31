namespace be.DTOs.InstructorFeedback;

public class InstructorFeedbackResponse
{
    public Guid Id { get; set; }

    public Guid SimulationSessionId { get; set; }

    public Guid InstructorId { get; set; }

    public string InstructorName { get; set; } = string.Empty;

    public string ScenarioName { get; set; } = string.Empty;

    public string OverallComments { get; set; } = string.Empty;

    public DateTime EvaluatedAt { get; set; }

    public List<string> Strengths { get; set; } = new();

    public List<string> ImprovementAreas { get; set; } = new();

    public List<SafetyRecommendationResponse> SafetyRecommendations { get; set; }
        = new();

    public List<ViolationExplanationResponse> ViolationExplanations { get; set; }
        = new();

    public List<RecommendedModuleResponse> RecommendedModules { get; set; }
        = new();
}