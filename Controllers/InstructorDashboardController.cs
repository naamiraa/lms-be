using be.Services.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace be.Controllers;

[ApiController]
[Route("api/dashboard/instructor")]
[Authorize(Roles = "INSTRUCTOR")]
public class InstructorDashboardController : ControllerBase
{
    private readonly IInstructorDashboardService _service;

    public InstructorDashboardController(
        IInstructorDashboardService service)
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
