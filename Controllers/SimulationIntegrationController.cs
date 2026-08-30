using be.DTOs.SimulationSession;
using be.Services.SimulationSession;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace be.Controllers;

[ApiController]
[Route("api/simulator")]
[Authorize(Roles = "ADMIN")]
public class SimulatorIntegrationController : ControllerBase
{
    private readonly ISimulationSessionService _service;

    public SimulatorIntegrationController(
        ISimulationSessionService service)
    {
        _service = service;
    }

    [HttpPost("sessions/{sessionId:guid}/start")]
    public async Task<IActionResult> Start(Guid sessionId)
    {
        try
        {
            var result =
                await _service.StartSessionAsync(sessionId);

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

    [HttpPost("sessions/{sessionId:guid}/complete")]
    public async Task<IActionResult> Complete(
        Guid sessionId,
        CompleteSimulationRequest request)
    {
        try
        {
            var result =
                await _service.CompleteSessionAsync(
                    sessionId,
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
}