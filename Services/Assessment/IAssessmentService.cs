using be.DTOs.Assessment;

namespace be.Services.Assessment;

public interface IAssessmentService
{
    Task<List<AssessmentResponse>> GetAllAsync();

    Task<List<AssessmentResponse>> GetByLearningModuleAsync(
        Guid learningModuleId);

    Task<AssessmentResponse?> GetByIdAsync(Guid id);

    Task<AssessmentResponse> CreateAsync(
        CreateAssessmentRequest request);

    Task<bool> UpdateAsync(
        Guid id,
        UpdateAssessmentRequest request);

    Task<bool> DeactivateAsync(Guid id);
}