namespace be.Models;

public class InstructorFeedback
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid SimulationSessionId { get; set; }

    public SimulationSession SimulationSession { get; set; } = null!;

    public Guid InstructorId { get; set; }

    public User Instructor { get; set; } = null!;

    public string OverallComments { get; set; } = string.Empty;

    public DateTime EvaluatedAt { get; set; } = DateTime.UtcNow;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<FeedbackStrength> Strengths { get; set; }
        = new List<FeedbackStrength>();

    public ICollection<FeedbackImprovementArea> ImprovementAreas { get; set; }
        = new List<FeedbackImprovementArea>();

    public ICollection<SafetyRecommendation> SafetyRecommendations { get; set; }
        = new List<SafetyRecommendation>();

    public ICollection<ViolationExplanation> ViolationExplanations { get; set; }
        = new List<ViolationExplanation>();

    public ICollection<FeedbackRecommendedModule> RecommendedModules { get; set; }
        = new List<FeedbackRecommendedModule>();
}