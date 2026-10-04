using be.DTOs.InstructorSession;

namespace be.Services.InstructorSession;

public interface IInstructorSessionService
{
    Task<List<InstructorSessionResponse>> GetCompletedSessionsAsync();
}