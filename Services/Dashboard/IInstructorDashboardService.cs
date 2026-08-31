using be.DTOs.Dashboard;

namespace be.Services.Dashboard;

public interface IInstructorDashboardService
{
    Task<InstructorDashboardResponse> GetDashboardAsync();
}