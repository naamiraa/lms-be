using be.DTOs.SimulationScenario;
using be.Services.SimulationScenario;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace be.Controllers;

[ApiController]
[Route("api/simulation-scenarios")]
public class SimulationScenarioController : ControllerBase
{
    private readonly ISimulationScenarioService _service;

    public SimulationScenarioController(
        ISimulationScenarioService service)
    {
        _service = service;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> GetById(Guid id)
    {
        var scenario = await _service.GetByIdAsync(id);

        if (scenario == null)
        {
            return NotFound(new
            {
                message = "Simulation scenario not found."
            });
        }

        return Ok(scenario);
    }

    [HttpGet("domain/{domainId:guid}")]
    [Authorize]
    public async Task<IActionResult> GetByDomain(Guid domainId)
    {
        return Ok(
            await _service.GetByDomainIdAsync(domainId));
    }

    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Create(
        CreateSimulationScenarioRequest request)
    {
        try
        {
            var scenario = await _service.CreateAsync(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = scenario.Id },
                scenario);
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
        UpdateSimulationScenarioRequest request)
    {
        try
        {
            var success =
                await _service.UpdateAsync(id, request);

            if (!success)
            {
                return NotFound(new
                {
                    message = "Simulation scenario not found."
                });
            }

            return Ok(new
            {
                message = "Simulation scenario updated successfully."
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
        var success = await _service.DeactivateAsync(id);

        if (!success)
        {
            return NotFound(new
            {
                message = "Simulation scenario not found."
            });
        }

        return Ok(new
        {
            message = "Simulation scenario deactivated successfully."
        });
    }
}