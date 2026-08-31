namespace be.DTOs.InstructorFeedback;

public class SafetyRecommendationRequest
{
    public string Description { get; set; } = string.Empty;

    public int Priority { get; set; }
}