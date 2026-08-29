using be.DTOs.LearningModule;

namespace be.Services.LearningModule;

public interface ILearningModuleService
{
    Task<List<LearningModuleResponse>> GetAllAsync();

    Task<List<LearningModuleResponse>> GetByTrainingDomainAsync(
        Guid trainingDomainId);

    Task<LearningModuleResponse?> GetByIdAsync(Guid id);

    Task<LearningModuleResponse> CreateAsync(
        CreateLearningModuleRequest request);

    Task<bool> UpdateAsync(
        Guid id,
        UpdateLearningModuleRequest request);

    Task<bool> DeactivateAsync(Guid id);
}