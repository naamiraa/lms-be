using be.Data;
using be.DTOs.Dashboard;
using be.Services.TrainingHistory;
using Microsoft.EntityFrameworkCore;

namespace be.Services.Dashboard;

public class ParticipantDashboardService
    : IParticipantDashboardService
{
    private readonly AppDbContext _context;
    private readonly ITrainingHistoryService _trainingHistoryService;

    public ParticipantDashboardService(
        AppDbContext context,
        ITrainingHistoryService trainingHistoryService)
    {
        _context = context;
        _trainingHistoryService = trainingHistoryService;
    }

    public async Task<ParticipantDashboardResponse>
        GetDashboardAsync(Guid userId)
    {
        var completedLearningContents =
            await _context.LearningProgresses
                .CountAsync(p =>
                    p.UserId == userId &&
                    p.IsCompleted);

        var totalLearningContents =
            await _context.LearningContents
                .CountAsync(c => c.IsActive);

        var learningProgressPercentage =
            totalLearningContents == 0
                ? 0
                : Math.Round(
                    (double)completedLearningContents /
                    totalLearningContents * 100,
                    2);

        var completedAssessments =
            await _context.AssessmentAttempts
                .CountAsync(a =>
                    a.UserId == userId &&
                    a.Status == "COMPLETED");

        var completedSimulations =
            await _context.SimulationSessions
                .CountAsync(s =>
                    s.UserId == userId &&
                    s.Status == "COMPLETED");

        var certificatesEarned =
            await _context.Certificates
                .CountAsync(c =>
                    c.UserId == userId);

        var history =
            await _trainingHistoryService
                .GetMyHistoryAsync(userId);

        var recentTrainings = history
            .OrderByDescending(h => h.CompletedAt)
            .Take(5)
            .ToList();

        return new ParticipantDashboardResponse
        {
            CompletedLearningContents =
                completedLearningContents,

            TotalLearningContents =
                totalLearningContents,

            LearningProgressPercentage =
                learningProgressPercentage,

            CompletedAssessments =
                completedAssessments,

            CompletedSimulations =
                completedSimulations,

            CertificatesEarned =
                certificatesEarned,

            LatestTraining =
                recentTrainings.FirstOrDefault(),

            RecentTrainings =
                recentTrainings
        };
    }
}
