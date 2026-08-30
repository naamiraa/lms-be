using be.DTOs.SimulationDomain;

namespace be.Services.SimulationDomain;

public interface ISimulationDomainService
{
    Task<List<SimulationDomainResponse>> GetAllAsync();

    Task<SimulationDomainResponse?> GetByIdAsync(Guid id);

    Task<SimulationDomainResponse> CreateAsync(
        CreateSimulationDomainRequest request);

    Task<bool> UpdateAsync(
        Guid id,
        UpdateSimulationDomainRequest request);

    Task<bool> DeactivateAsync(Guid id);
}