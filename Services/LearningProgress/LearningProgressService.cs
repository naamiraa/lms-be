using be.Data;
using be.DTOs.LearningProgress;
using Microsoft.EntityFrameworkCore;

namespace be.Services.LearningProgress;

public class LearningProgressService : ILearningProgressService
{
    private readonly AppDbContext _context;

    public LearningProgressService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<LearningProgressResponse> CompleteContentAsync(
        Guid userId,
        Guid learningContentId)
    {
        var content = await _context.LearningContents
            .FirstOrDefaultAsync(c =>
                c.Id == learningContentId &&
                c.IsActive);

        if (content == null)
        {
            throw new ArgumentException(
                "Learning content not found or inactive.");
        }

        var progress = await _context.LearningProgresses
            .FirstOrDefaultAsync(p =>
                p.UserId == userId &&
                p.LearningContentId == learningContentId);

        if (progress == null)
        {
            progress = new Models.LearningProgress
            {
                UserId = userId,
                LearningContentId = learningContentId,
                IsCompleted = true,
                CompletedAt = DateTime.UtcNow
            };

            _context.LearningProgresses.Add(progress);
        }
        else if (!progress.IsCompleted)
        {
            progress.IsCompleted = true;
            progress.CompletedAt = DateTime.UtcNow;
            progress.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        return new LearningProgressResponse
        {
            LearningContentId = content.Id,
            LearningContentTitle = content.Title,
            IsCompleted = progress.IsCompleted,
            CompletedAt = progress.CompletedAt
        };
    }

    public async Task<ModuleProgressResponse?> GetModuleProgressAsync(
        Guid userId,
        Guid learningModuleId)
    {
        var module = await _context.LearningModules
            .FirstOrDefaultAsync(m =>
                m.Id == learningModuleId &&
                m.IsActive);

        if (module == null)
        {
            return null;
        }

        var contents = await _context.LearningContents
            .Where(c =>
                c.LearningModuleId == learningModuleId &&
                c.IsActive)
            .OrderBy(c => c.Order)
            .Select(c => new
            {
                c.Id,
                c.Title
            })
            .ToListAsync();

        var contentIds = contents
            .Select(c => c.Id)
            .ToList();

        var progresses = await _context.LearningProgresses
            .Where(p =>
                p.UserId == userId &&
                contentIds.Contains(p.LearningContentId))
            .ToListAsync();

        var contentProgress = contents.Select(content =>
        {
            var progress = progresses.FirstOrDefault(p =>
                p.LearningContentId == content.Id);

            return new LearningProgressResponse
            {
                LearningContentId = content.Id,
                LearningContentTitle = content.Title,
                IsCompleted = progress?.IsCompleted ?? false,
                CompletedAt = progress?.CompletedAt
            };
        }).ToList();

        var totalContents = contents.Count;

        var completedContents =
            contentProgress.Count(p => p.IsCompleted);

        var percentage = totalContents == 0
            ? 0
            : Math.Round(
                (double)completedContents / totalContents * 100,
                2);

        return new ModuleProgressResponse
        {
            LearningModuleId = module.Id,
            LearningModuleName = module.Name,
            TotalContents = totalContents,
            CompletedContents = completedContents,
            ProgressPercentage = percentage,

            IsCompleted =
                totalContents > 0 &&
                completedContents == totalContents,

            Contents = contentProgress
        };
    }
}