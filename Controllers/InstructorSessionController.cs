using be.Services.InstructorSession;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace be.Controllers;

[ApiController]
[Route("api/instructor/sessions")]
[Authorize(Roles = "INSTRUCTOR")]
public class InstructorSessionController : ControllerBase
{
    private readonly IInstructorSessionService _service;

    public InstructorSessionController(
        IInstructorSessionService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetCompletedSessions()
    {
        var result =
            await _service.GetCompletedSessionsAsync();

        return Ok(result);
    }
}