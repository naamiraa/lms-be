using be.DTOs.TrainingDomain;

namespace be.Services.TrainingDomain;

public interface ITrainingDomainService
{
    Task<List<TrainingDomainResponse>> GetAllAsync();

    Task<TrainingDomainResponse?> GetByIdAsync(Guid id);

    Task<TrainingDomainResponse> CreateAsync(
        CreateTrainingDomainRequest request);

    Task<bool> UpdateAsync(
        Guid id,
        UpdateTrainingDomainRequest request);

    Task<bool> DeactivateAsync(Guid id);
}