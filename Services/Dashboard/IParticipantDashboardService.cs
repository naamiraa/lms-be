using be.DTOs.Dashboard;

namespace be.Services.Dashboard;

public interface IParticipantDashboardService
{
    Task<ParticipantDashboardResponse> GetDashboardAsync(
        Guid userId);
}
