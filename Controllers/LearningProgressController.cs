using System.Security.Claims;
using be.Services.LearningProgress;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace be.Controllers;

[ApiController]
[Route("api/learning-progress")]
[Authorize]
public class LearningProgressController : ControllerBase
{
    private readonly ILearningProgressService _learningProgressService;

    public LearningProgressController(
        ILearningProgressService learningProgressService)
    {
        _learningProgressService = learningProgressService;
    }

    [HttpPost("contents/{learningContentId:guid}/complete")]
    [Authorize(Roles = "PARTICIPANT")]
    public async Task<IActionResult> CompleteContent(
        Guid learningContentId)
    {
        var userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized(new
            {
                message = "Invalid user token."
            });
        }

        try
        {
            var progress =
                await _learningProgressService.CompleteContentAsync(
                    userId.Value,
                    learningContentId
                );

            return Ok(progress);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    [HttpGet("modules/{learningModuleId:guid}")]
    [Authorize(Roles = "PARTICIPANT")]
    public async Task<IActionResult> GetModuleProgress(
        Guid learningModuleId)
    {
        var userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized(new
            {
                message = "Invalid user token."
            });
        }

        var progress =
            await _learningProgressService.GetModuleProgressAsync(
                userId.Value,
                learningModuleId
            );

        if (progress == null)
        {
            return NotFound(new
            {
                message = "Learning module not found."
            });
        }

        return Ok(progress);
    }

    private Guid? GetCurrentUserId()
    {
        var userIdClaim =
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (Guid.TryParse(userIdClaim, out var userId))
        {
            return userId;
        }

        return null;
    }
}