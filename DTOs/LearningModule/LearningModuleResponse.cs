namespace be.DTOs.LearningModule;

public class LearningModuleResponse
{
    public Guid Id { get; set; }

    public Guid TrainingDomainId { get; set; }

    public string TrainingDomainName { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int Order { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}