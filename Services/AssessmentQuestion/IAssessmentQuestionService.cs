using be.DTOs.AssessmentQuestion;

namespace be.Services.AssessmentQuestion;

public interface IAssessmentQuestionService
{
    Task<List<AssessmentQuestionResponse>> GetByAssessmentAsync(
        Guid assessmentId);

    Task<AssessmentQuestionResponse?> GetByIdAsync(Guid id);

    Task<AssessmentQuestionResponse> CreateAsync(
        CreateAssessmentQuestionRequest request);

    Task<bool> UpdateAsync(
        Guid id,
        UpdateAssessmentQuestionRequest request);

    Task<bool> DeactivateAsync(Guid id);
}