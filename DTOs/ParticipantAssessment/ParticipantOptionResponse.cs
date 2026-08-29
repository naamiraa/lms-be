namespace be.DTOs.ParticipantAssessment;

public class ParticipantOptionResponse
{
    public Guid Id { get; set; }

    public string OptionText { get; set; } = string.Empty;

    public int Order { get; set; }
}