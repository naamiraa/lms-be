using System.Security.Claims;
using be.DTOs.SimulationSession;
using be.Services.SimulationSession;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace be.Controllers;

[ApiController]
[Route("api/simulation-sessions")]
[Authorize(Roles = "PARTICIPANT")]
public class SimulationSessionController : ControllerBase
{
    private readonly ISimulationSessionService _service;

    public SimulationSessionController(
        ISimulationSessionService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateSimulationSessionRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();

            var result =
                await _service.CreateSessionAsync(
                    userId,
                    request);

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

    [HttpGet("{sessionId:guid}")]
    public async Task<IActionResult> GetById(
        Guid sessionId)
    {
        var userId = GetCurrentUserId();

        var session =
            await _service.GetByIdAsync(
                userId,
                sessionId);

        if (session == null)
        {
            return NotFound(new
            {
                message = "Simulation session not found."
            });
        }

        return Ok(session);
    }

    private Guid GetCurrentUserId()
    {
        var userIdValue =
            User.FindFirstValue(
                ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(
            userIdValue,
            out var userId))
        {
            throw new UnauthorizedAccessException(
                "Invalid user identifier.");
        }

        return userId;
    }
}