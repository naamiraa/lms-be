namespace be.DTOs.LearningModule;

public class CreateLearningModuleRequest
{
    public Guid TrainingDomainId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int Order { get; set; }
}