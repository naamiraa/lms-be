namespace be.DTOs.InstructorFeedback;

public class RecommendedModuleRequest
{
    public Guid LearningModuleId { get; set; }

    public string? RecommendationType { get; set; }

    public int Order { get; set; }
}