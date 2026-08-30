using System.Security.Claims;
using be.Services.PerformanceEvaluation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace be.Controllers;

[ApiController]
[Route("api/performance-evaluations")]
[Authorize(Roles = "PARTICIPANT")]
public class PerformanceEvaluationController
    : ControllerBase
{
    private readonly IPerformanceEvaluationService _service;

    public PerformanceEvaluationController(
        IPerformanceEvaluationService service)
    {
        _service = service;
    }

    [HttpGet("session/{sessionId:guid}")]
    public async Task<IActionResult> GetBySession(
        Guid sessionId)
    {
        try
        {
            var userIdValue =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(
                userIdValue,
                out var userId))
            {
                return Unauthorized();
            }

            var result =
                await _service.GetBySessionIdAsync(
                    userId,
                    sessionId);

            if (result == null)
            {
                return NotFound(new
                {
                    message =
                        "Performance evaluation not found."
                });
            }

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }
}