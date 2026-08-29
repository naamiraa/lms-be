using be.DTOs.LearningProgress;

namespace be.Services.LearningProgress;

public interface ILearningProgressService
{
    Task<LearningProgressResponse> CompleteContentAsync(
        Guid userId,
        Guid learningContentId);

    Task<ModuleProgressResponse?> GetModuleProgressAsync(
        Guid userId,
        Guid learningModuleId);
}