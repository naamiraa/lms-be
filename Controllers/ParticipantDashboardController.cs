using System.Security.Claims;
using be.Services.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace be.Controllers;

[ApiController]
[Route("api/dashboard/participant")]
[Authorize(Roles = "PARTICIPANT")]
public class ParticipantDashboardController : ControllerBase
{
    private readonly IParticipantDashboardService _service;

    public ParticipantDashboardController(
        IParticipantDashboardService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetDashboard()
    {
        var userId = GetCurrentUserId();

        var result =
            await _service.GetDashboardAsync(userId);

        return Ok(result);
    }

    private Guid GetCurrentUserId()
    {
        var value =
            User.FindFirstValue(
                ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(value, out var userId))
        {
            throw new UnauthorizedAccessException(
                "Invalid user identifier.");
        }

        return userId;
    }
}
