using be.Data;
using be.DTOs.LearningModule;
using Microsoft.EntityFrameworkCore;

namespace be.Services.LearningModule;

public class LearningModuleService : ILearningModuleService
{
    private readonly AppDbContext _context;

    public LearningModuleService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<LearningModuleResponse>> GetAllAsync()
    {
        return await _context.LearningModules
            .Where(m => m.IsActive)
            .OrderBy(m => m.Order)
            .Select(m => new LearningModuleResponse
            {
                Id = m.Id,
                TrainingDomainId = m.TrainingDomainId,
                TrainingDomainName = m.TrainingDomain.Name,
                Name = m.Name,
                Description = m.Description,
                Order = m.Order,
                IsActive = m.IsActive,
                CreatedAt = m.CreatedAt,
                UpdatedAt = m.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task<List<LearningModuleResponse>>
        GetByTrainingDomainAsync(Guid trainingDomainId)
    {
        return await _context.LearningModules
            .Where(m =>
                m.TrainingDomainId == trainingDomainId &&
                m.IsActive)
            .OrderBy(m => m.Order)
            .Select(m => new LearningModuleResponse
            {
                Id = m.Id,
                TrainingDomainId = m.TrainingDomainId,
                TrainingDomainName = m.TrainingDomain.Name,
                Name = m.Name,
                Description = m.Description,
                Order = m.Order,
                IsActive = m.IsActive,
                CreatedAt = m.CreatedAt,
                UpdatedAt = m.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task<LearningModuleResponse?> GetByIdAsync(Guid id)
    {
        return await _context.LearningModules
            .Where(m => m.Id == id)
            .Select(m => new LearningModuleResponse
            {
                Id = m.Id,
                TrainingDomainId = m.TrainingDomainId,
                TrainingDomainName = m.TrainingDomain.Name,
                Name = m.Name,
                Description = m.Description,
                Order = m.Order,
                IsActive = m.IsActive,
                CreatedAt = m.CreatedAt,
                UpdatedAt = m.UpdatedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task<LearningModuleResponse> CreateAsync(
        CreateLearningModuleRequest request)
    {
        var domain = await _context.TrainingDomains
            .FirstOrDefaultAsync(d =>
                d.Id == request.TrainingDomainId &&
                d.IsActive);

        if (domain == null)
        {
            throw new ArgumentException(
                "Training domain not found or inactive.");
        }

        var module = new Models.LearningModule
        {
            TrainingDomainId = request.TrainingDomainId,
            Name = request.Name,
            Description = request.Description,
            Order = request.Order
        };

        _context.LearningModules.Add(module);
        await _context.SaveChangesAsync();

        return new LearningModuleResponse
        {
            Id = module.Id,
            TrainingDomainId = module.TrainingDomainId,
            TrainingDomainName = domain.Name,
            Name = module.Name,
            Description = module.Description,
            Order = module.Order,
            IsActive = module.IsActive,
            CreatedAt = module.CreatedAt,
            UpdatedAt = module.UpdatedAt
        };
    }

    public async Task<bool> UpdateAsync(
        Guid id,
        UpdateLearningModuleRequest request)
    {
        var module = await _context.LearningModules
            .FirstOrDefaultAsync(m => m.Id == id);

        if (module == null)
        {
            return false;
        }

        var domainExists = await _context.TrainingDomains
            .AnyAsync(d =>
                d.Id == request.TrainingDomainId &&
                d.IsActive);

        if (!domainExists)
        {
            throw new ArgumentException(
                "Training domain not found or inactive.");
        }

        module.TrainingDomainId = request.TrainingDomainId;
        module.Name = request.Name;
        module.Description = request.Description;
        module.Order = request.Order;
        module.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeactivateAsync(Guid id)
    {
        var module = await _context.LearningModules
            .FirstOrDefaultAsync(m => m.Id == id);

        if (module == null)
        {
            return false;
        }

        module.IsActive = false;
        module.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return true;
    }
}