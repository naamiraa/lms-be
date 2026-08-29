using be.DTOs.LearningContent;

namespace be.Services.LearningContent;

public interface ILearningContentService
{
    Task<List<LearningContentResponse>> GetAllAsync();

    Task<List<LearningContentResponse>> GetByLearningModuleAsync(
        Guid learningModuleId);

    Task<LearningContentResponse?> GetByIdAsync(Guid id);

    Task<LearningContentResponse> CreateAsync(
        CreateLearningContentRequest request);

    Task<bool> UpdateAsync(
        Guid id,
        UpdateLearningContentRequest request);

    Task<bool> DeactivateAsync(Guid id);
}