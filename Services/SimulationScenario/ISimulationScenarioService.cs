using be.DTOs.SimulationScenario;

namespace be.Services.SimulationScenario;

public interface ISimulationScenarioService
{
    Task<List<SimulationScenarioResponse>> GetAllAsync();

    Task<List<SimulationScenarioResponse>> GetByDomainIdAsync(Guid domainId);

    Task<SimulationScenarioResponse?> GetByIdAsync(Guid id);

    Task<SimulationScenarioResponse> CreateAsync(
        CreateSimulationScenarioRequest request);

    Task<bool> UpdateAsync(
        Guid id,
        UpdateSimulationScenarioRequest request);

    Task<bool> DeactivateAsync(Guid id);
}