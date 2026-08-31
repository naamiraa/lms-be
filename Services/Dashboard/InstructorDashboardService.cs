using be.Data;
using be.DTOs.Dashboard;
using be.Services.Evaluation;
using Microsoft.EntityFrameworkCore;

namespace be.Services.Dashboard;

public class InstructorDashboardService
    : IInstructorDashboardService
{
    private readonly AppDbContext _context;
    private readonly IEvaluationCalculator _evaluationCalculator;

    public InstructorDashboardService(
        AppDbContext context,
        IEvaluationCalculator evaluationCalculator)
    {
        _context = context;
        _evaluationCalculator = evaluationCalculator;
    }

    public async Task<InstructorDashboardResponse>
        GetDashboardAsync()
    {
        var completedSessions =
            await _context.SimulationSessions
                .Where(s =>
                    s.Status == "COMPLETED" &&
                    s.Result != null)
                .Include(s => s.User)
                .Include(s => s.Result)
                .Include(s => s.AssessmentAttempt)
                .Include(s => s.SimulationScenario)
                .Include(s => s.InstructorFeedback)
                .ToListAsync();

        var completedSimulations =
            completedSessions.Count;

        var pendingFeedback =
            completedSessions.Count(s =>
                s.InstructorFeedback == null);

        var evaluatedSessions =
            completedSessions.Count(s =>
                s.InstructorFeedback != null);

        var averageSimulatorScore =
            completedSimulations == 0
                ? 0
                : Math.Round(
                    completedSessions.Average(
                        s => s.Result!.OverallSimulatorScore),
                    2);

        var recentSessions =
            completedSessions
                .OrderByDescending(s =>
                    s.Result!.CreatedAt)
                .Take(5)
                .Select(s =>
                {
                    var theoryScore =
                        s.AssessmentAttempt.Score ?? 0;

                    var simulatorScore =
                        s.Result!.OverallSimulatorScore;

                    var overallScore =
                        _evaluationCalculator
                            .CalculateOverallScore(
                                theoryScore,
                                simulatorScore);

                    return new InstructorRecentSessionResponse
                    {
                        SimulationSessionId = s.Id,

                        ParticipantName =
                            s.User.Name,

                        ScenarioName =
                            s.SimulationScenario.Name,

                        SimulatorScore =
                            simulatorScore,

                        OverallScore =
                            overallScore,

                        Result =
                            _evaluationCalculator
                                .DetermineResult(overallScore),

                        CompletedAt =
                            s.Result.CreatedAt,

                        HasInstructorFeedback =
                            s.InstructorFeedback != null
                    };
                })
                .ToList();

        return new InstructorDashboardResponse
        {
            CompletedSimulations =
                completedSimulations,

            PendingFeedback =
                pendingFeedback,

            EvaluatedSessions =
                evaluatedSessions,

            AverageSimulatorScore =
                averageSimulatorScore,

            RecentSessions =
                recentSessions
        };
    }
}
