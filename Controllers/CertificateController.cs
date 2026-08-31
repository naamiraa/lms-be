using System.Security.Claims;
using be.Services.Certificate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace be.Controllers;

[ApiController]
[Route("api/certificates")]
[Authorize(Roles = "PARTICIPANT")]
public class CertificateController : ControllerBase
{
    private readonly ICertificateService _service;

    public CertificateController(
        ICertificateService service)
    {
        _service = service;
    }

    [HttpPost("session/{sessionId:guid}")]
    public async Task<IActionResult> Issue(Guid sessionId)
    {
        try
        {
            var userId = GetCurrentUserId();

            var result = await _service.IssueAsync(
                userId,
                sessionId);

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetMine()
    {
        var result = await _service
            .GetMyCertificatesAsync(
                GetCurrentUserId());

        return Ok(result);
    }

    [HttpGet("session/{sessionId:guid}")]
    public async Task<IActionResult> GetBySession(
        Guid sessionId)
    {
        var result =
            await _service.GetBySessionAsync(
                GetCurrentUserId(),
                sessionId);

        if (result == null)
        {
            return NotFound(new
            {
                message = "Certificate not found."
            });
        }

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
