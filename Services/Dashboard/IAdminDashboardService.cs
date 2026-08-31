using be.DTOs.Dashboard;

namespace be.Services.Dashboard;

public interface IAdminDashboardService
{
    Task<AdminDashboardResponse> GetDashboardAsync();
}