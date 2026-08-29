using be.DTOs.LearningContent;
using be.Services.LearningContent;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace be.Controllers;

[ApiController]
[Route("api/learning-contents")]
public class LearningContentsController : ControllerBase
{
    private readonly ILearningContentService _learningContentService;

    public LearningContentsController(
        ILearningContentService learningContentService)
    {
        _learningContentService = learningContentService;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll()
    {
        var contents =
            await _learningContentService.GetAllAsync();

        return Ok(contents);
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> GetById(Guid id)
    {
        var content =
            await _learningContentService.GetByIdAsync(id);

        if (content == null)
        {
            return NotFound(new
            {
                message = "Learning content not found."
            });
        }

        return Ok(content);
    }

    [HttpGet("learning-module/{learningModuleId:guid}")]
    [Authorize]
    public async Task<IActionResult> GetByLearningModule(
        Guid learningModuleId)
    {
        var contents =
            await _learningContentService
                .GetByLearningModuleAsync(learningModuleId);

        return Ok(contents);
    }

    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Create(
        CreateLearningContentRequest request)
    {
        try
        {
            var content =
                await _learningContentService.CreateAsync(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = content.Id },
                content
            );
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateLearningContentRequest request)
    {
        try
        {
            var success =
                await _learningContentService
                    .UpdateAsync(id, request);

            if (!success)
            {
                return NotFound(new
                {
                    message = "Learning content not found."
                });
            }

            return Ok(new
            {
                message = "Learning content updated successfully."
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        var success =
            await _learningContentService.DeactivateAsync(id);

        if (!success)
        {
            return NotFound(new
            {
                message = "Learning content not found."
            });
        }

        return Ok(new
        {
            message = "Learning content deactivated successfully."
        });
    }
}