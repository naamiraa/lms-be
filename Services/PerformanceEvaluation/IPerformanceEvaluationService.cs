using be.DTOs.PerformanceEvaluation;

namespace be.Services.PerformanceEvaluation;

public interface IPerformanceEvaluationService
{
    Task<PerformanceEvaluationResponse?> GetBySessionIdAsync(
        Guid userId,
        Guid sessionId);
}