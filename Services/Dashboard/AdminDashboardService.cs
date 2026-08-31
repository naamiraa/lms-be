using be.Data;
using be.DTOs.Dashboard;
using be.Services.Evaluation;
using Microsoft.EntityFrameworkCore;

namespace be.Services.Dashboard;

public class AdminDashboardService : IAdminDashboardService
{
    private readonly AppDbContext _context;
    private readonly IEvaluationCalculator _evaluationCalculator;

    public AdminDashboardService(
        AppDbContext context,
        IEvaluationCalculator evaluationCalculator)
    {
        _context = context;
        _evaluationCalculator = evaluationCalculator;
    }

    public async Task<AdminDashboardResponse> GetDashboardAsync()
    {
        var totalParticipants =
            await _context.UserRoles
                .CountAsync(ur =>
                    ur.Role.Name == "PARTICIPANT");

        var totalInstructors =
            await _context.UserRoles
                .CountAsync(ur =>
                    ur.Role.Name == "INSTRUCTOR");

        var pendingUserApprovals =
            await _context.Users
                .CountAsync(u =>
                    u.ApprovalStatus == "PENDING");

        var completedSessions =
            await _context.SimulationSessions
                .Where(s =>
                    s.Status == "COMPLETED" &&
                    s.Result != null &&
                    s.AssessmentAttempt.Score != null)
                .Include(s => s.Result)
                .Include(s => s.AssessmentAttempt)
                .Include(s => s.SimulationScenario)
                .ToListAsync();

        var completedSimulations =
            completedSessions.Count;

        var certificatesIssued =
            await _context.Certificates.CountAsync();

        var overallScores = completedSessions
            .Select(s =>
                _evaluationCalculator.CalculateOverallScore(
                    s.AssessmentAttempt.Score!.Value,
                    s.Result!.OverallSimulatorScore))
            .ToList();

        var passedCount =
            overallScores.Count(score =>
                _evaluationCalculator.DetermineResult(score)
                    == "PASS");

        var failedCount =
            completedSimulations - passedCount;


        var scenarioPerformance =
            completedSessions
                .GroupBy(s => new
                {
                    s.SimulationScenarioId,
                    s.SimulationScenario.Name
                })
                .Select(group =>
                {
                    var scores = group
                        .Select(s =>
                            _evaluationCalculator
                                .CalculateOverallScore(
                                    s.AssessmentAttempt.Score!.Value,
                                    s.Result!.OverallSimulatorScore))
                        .ToList();

                    var scenarioPassedCount =
                        scores.Count(score =>
                            _evaluationCalculator
                                .DetermineResult(score) == "PASS");

                    return new ScenarioPerformanceResponse
                    {
                        ScenarioId =
                            group.Key.SimulationScenarioId,

                        ScenarioName =
                            group.Key.Name,

                        CompletedSimulations =
                            scores.Count,

                        AverageOverallScore =
                            scores.Count == 0
                                ? 0
                                : Math.Round(
                                    scores.Average(),
                                    2),

                        PassRate =
                            scores.Count == 0
                                ? 0
                                : Math.Round(
                                    (double)scenarioPassedCount /
                                    scores.Count * 100,
                                    2)
                    };
                })
                .OrderByDescending(s =>
                    s.CompletedSimulations)
                .ToList();


        var passRate =
            completedSimulations == 0
                ? 0
                : Math.Round(
                    (double)passedCount /
                    completedSimulations * 100,
                    2);

        var averageOverallScore =
            overallScores.Count == 0
                ? 0
                : Math.Round(
                    overallScores.Average(),
                    2);

        return new AdminDashboardResponse
        {
            TotalParticipants = totalParticipants,

            TotalInstructors = totalInstructors,

            PendingUserApprovals = pendingUserApprovals,

            CompletedSimulations = completedSimulations,

            CertificatesIssued = certificatesIssued,

            PassedSimulations = passedCount,

            FailedSimulations = failedCount,

            PassRate = passRate,

            AverageOverallScore = averageOverallScore,

            ScenarioPerformance = scenarioPerformance
        };
    }
}
