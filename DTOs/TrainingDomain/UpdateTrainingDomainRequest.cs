namespace be.DTOs.TrainingDomain;

public class UpdateTrainingDomainRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}