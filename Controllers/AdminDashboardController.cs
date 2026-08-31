using be.Services.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace be.Controllers;

[ApiController]
[Route("api/dashboard/admin")]
[Authorize(Roles = "ADMIN")]
public class AdminDashboardController : ControllerBase
{
    private readonly IAdminDashboardService _service;

    public AdminDashboardController(
        IAdminDashboardService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetDashboard()
    {
        var result =
            await _service.GetDashboardAsync();

        return Ok(result);
    }
}
