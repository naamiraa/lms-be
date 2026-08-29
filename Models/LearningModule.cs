namespace be.Models;

public class LearningModule
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid TrainingDomainId { get; set; }

    public TrainingDomain TrainingDomain { get; set; } = null!;

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int Order { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<LearningContent> Contents { get; set; }
        = new List<LearningContent>();

    public ICollection<Assessment> Assessments { get; set; }
    = new List<Assessment>();
}