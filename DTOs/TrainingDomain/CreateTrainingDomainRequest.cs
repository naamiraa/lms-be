namespace be.DTOs.TrainingDomain;

public class CreateTrainingDomainRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}