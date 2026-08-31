namespace be.DTOs.InstructorFeedback;

public class RecommendedModuleResponse
{
    public Guid LearningModuleId { get; set; }

    public string LearningModuleName { get; set; } = string.Empty;

    public string? RecommendationType { get; set; }

    public int Order { get; set; }
}