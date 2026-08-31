using be.DTOs.InstructorFeedback;

namespace be.Services.InstructorFeedback;

public interface IInstructorFeedbackService
{
    Task<InstructorFeedbackResponse> CreateAsync(
        Guid instructorId,
        CreateInstructorFeedbackRequest request);

    Task<InstructorFeedbackResponse?> GetBySessionIdAsync(
        Guid userId,
        Guid sessionId,
        bool isInstructor);
}