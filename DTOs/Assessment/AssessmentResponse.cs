namespace be.DTOs.Assessment;

public class AssessmentResponse
{
    public Guid Id { get; set; }

    public Guid LearningModuleId { get; set; }

    public string LearningModuleName { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int DurationMinutes { get; set; }

    public double PassingScore { get; set; }

    public int MaxAttempts { get; set; }

    public int QuestionCount { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}