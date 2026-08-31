using be.DTOs.TrainingHistory;

namespace be.Services.TrainingHistory;

public interface ITrainingHistoryService
{
    Task<List<TrainingHistoryItemResponse>> GetMyHistoryAsync(Guid userId);
    Task<TrainingHistoryDetailResponse?> GetDetailAsync(
        Guid userId,
        Guid sessionId
    );
}