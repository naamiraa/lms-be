namespace be.DTOs.LearningProgress;

public class ModuleProgressResponse
{
    public Guid LearningModuleId { get; set; }

    public string LearningModuleName { get; set; } = string.Empty;

    public int TotalContents { get; set; }

    public int CompletedContents { get; set; }

    public double ProgressPercentage { get; set; }

    public bool IsCompleted { get; set; }

    public List<LearningProgressResponse> Contents { get; set; } = new();
}