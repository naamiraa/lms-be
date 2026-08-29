using be.DTOs.ParticipantAssessment;

namespace be.Services.ParticipantAssessment;

public interface IParticipantAssessmentService
{
    Task<StartAssessmentResponse> StartAssessmentAsync(
        Guid userId,
        Guid assessmentId);

    Task<SaveAssessmentAnswerResponse> SaveAnswerAsync(
        Guid userId,
        Guid attemptId,
        Guid questionId,
        SaveAssessmentAnswerRequest request);

    Task<SubmitAssessmentResponse> SubmitAssessmentAsync(
        Guid userId,
        Guid attemptId);
}