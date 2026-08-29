namespace be.DTOs.Assessment;

public class CreateAssessmentRequest
{
    public Guid LearningModuleId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Type { get; set; } = "POST_TEST";

    public string? Description { get; set; }

    public int DurationMinutes { get; set; }

    public double PassingScore { get; set; }

    public int MaxAttempts { get; set; }
}