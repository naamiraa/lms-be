using System.Security.Claims;
using be.DTOs.InstructorFeedback;
using be.Services.InstructorFeedback;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace be.Controllers;

[ApiController]
[Route("api/instructor-feedback")]
[Authorize]
public class InstructorFeedbackController : ControllerBase
{
    private readonly IInstructorFeedbackService _service;

    public InstructorFeedbackController(
        IInstructorFeedbackService service)
    {
        _service = service;
    }

    [HttpPost]
    [Authorize(Roles = "INSTRUCTOR")]
    public async Task<IActionResult> Create(
        CreateInstructorFeedbackRequest request)
    {
        try
        {
            var instructorId = GetCurrentUserId();

            var result = await _service.CreateAsync(
                instructorId,
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

    [HttpGet("session/{sessionId:guid}")]
    [Authorize(Roles = "PARTICIPANT,INSTRUCTOR")]
    public async Task<IActionResult> GetBySession(
        Guid sessionId)
    {
        var userId = GetCurrentUserId();

        var isInstructor = User.IsInRole("INSTRUCTOR");

        var result = await _service.GetBySessionIdAsync(
            userId,
            sessionId,
            isInstructor);

        if (result == null)
        {
            return NotFound(new
            {
                message = "Instructor feedback not found."
            });
        }

        return Ok(result);
    }

    private Guid GetCurrentUserId()
    {
        var value =
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(value, out var userId))
        {
            throw new UnauthorizedAccessException(
                "Invalid user identifier.");
        }

        return userId;
    }
}