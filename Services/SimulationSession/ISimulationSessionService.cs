using be.DTOs.SimulationSession;

namespace be.Services.SimulationSession;

public interface ISimulationSessionService
{
    Task<LaunchSimulationResponse> CreateSessionAsync(
        Guid userId,
        CreateSimulationSessionRequest request);

    Task<SimulationSessionResponse?> GetByIdAsync(
        Guid userId,
        Guid sessionId);

    Task<StartSimulationResponse> StartSessionAsync(
        Guid sessionId);

    Task<CompleteSimulationResponse> CompleteSessionAsync(
        Guid sessionId,
        CompleteSimulationRequest request);
}