using be.DTOs.LearningModule;
using be.Services.LearningModule;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace be.Controllers;

[ApiController]
[Route("api/learning-modules")]
public class LearningModulesController : ControllerBase
{
    private readonly ILearningModuleService _learningModuleService;

    public LearningModulesController(
        ILearningModuleService learningModuleService)
    {
        _learningModuleService = learningModuleService;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll()
    {
        var modules = await _learningModuleService.GetAllAsync();

        return Ok(modules);
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> GetById(Guid id)
    {
        var module = await _learningModuleService.GetByIdAsync(id);

        if (module == null)
        {
            return NotFound(new
            {
                message = "Learning module not found."
            });
        }

        return Ok(module);
    }

    [HttpGet("training-domain/{trainingDomainId:guid}")]
    [Authorize]
    public async Task<IActionResult> GetByTrainingDomain(
        Guid trainingDomainId)
    {
        var modules =
            await _learningModuleService
                .GetByTrainingDomainAsync(trainingDomainId);

        return Ok(modules);
    }

    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Create(
        CreateLearningModuleRequest request)
    {
        try
        {
            var module =
                await _learningModuleService.CreateAsync(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = module.Id },
                module
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
        UpdateLearningModuleRequest request)
    {
        try
        {
            var success =
                await _learningModuleService.UpdateAsync(id, request);

            if (!success)
            {
                return NotFound(new
                {
                    message = "Learning module not found."
                });
            }

            return Ok(new
            {
                message = "Learning module updated successfully."
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
            await _learningModuleService.DeactivateAsync(id);

        if (!success)
        {
            return NotFound(new
            {
                message = "Learning module not found."
            });
        }

        return Ok(new
        {
            message = "Learning module deactivated successfully."
        });
    }
}