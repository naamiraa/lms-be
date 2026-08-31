namespace be.DTOs.InstructorFeedback;

public class CreateInstructorFeedbackRequest
{
    public Guid SimulationSessionId { get; set; }

    public string OverallComments { get; set; } = string.Empty;

    public List<string> Strengths { get; set; } = new();

    public List<string> ImprovementAreas { get; set; } = new();

    public List<SafetyRecommendationRequest> SafetyRecommendations { get; set; }
        = new();

    public List<ViolationExplanationRequest> ViolationExplanations { get; set; }
        = new();

    public List<RecommendedModuleRequest> RecommendedModules { get; set; }
        = new();
}