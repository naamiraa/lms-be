using System.Security.Claims;
using be.Services.TrainingHistory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace be.Controllers;

[ApiController]
[Route("api/training-history")]
[Authorize(Roles = "PARTICIPANT")]
public class TrainingHistoryController : ControllerBase
{
    private readonly ITrainingHistoryService _service;

    public TrainingHistoryController(
        ITrainingHistoryService service)
    {
        _service = service;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMyHistory()
    {
        var userIdValue =
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdValue, out var userId))
        {
            return Unauthorized(new
            {
                message = "Invalid user identifier."
            });
        }

        var result =
            await _service.GetMyHistoryAsync(userId);

        return Ok(result);
    }


    [HttpGet("{sessionId:guid}")]
    public async Task<IActionResult> GetDetail(
        Guid sessionId)
    {
        var userIdValue =
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdValue, out var userId))
        {
            return Unauthorized(new
            {
                message = "Invalid user identifier."
            });
        }

        var result =
            await _service.GetDetailAsync(
                userId,
                sessionId);

        if (result == null)
        {
            return NotFound(new
            {
                message = "Training history not found."
            });
        }

        return Ok(result);
    }

}
