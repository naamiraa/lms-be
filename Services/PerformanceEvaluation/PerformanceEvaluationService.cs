using be.Data;
using be.DTOs.PerformanceEvaluation;
using Microsoft.EntityFrameworkCore;

namespace be.Services.PerformanceEvaluation;

public class PerformanceEvaluationService
    : IPerformanceEvaluationService
{
    private readonly AppDbContext _context;

    public PerformanceEvaluationService(
        AppDbContext context)
    {
        _context = context;
    }

    public async Task<PerformanceEvaluationResponse?>
        GetBySessionIdAsync(
            Guid userId,
            Guid sessionId)
    {
        var session = await _context.SimulationSessions
            .Include(s => s.AssessmentAttempt)
            .Include(s => s.Result)
            .Include(s => s.Events)
            .Include(s => s.SimulationScenario)
                .ThenInclude(s => s.SimulationDomain)
            .FirstOrDefaultAsync(s =>
                s.Id == sessionId &&
                s.UserId == userId);

        if (session == null)
        {
            return null;
        }

        if (session.Status != "COMPLETED")
        {
            throw new ArgumentException(
                "Performance evaluation is only available after simulation is completed.");
        }

        if (session.Result == null)
        {
            throw new ArgumentException(
                "Simulation result is not available.");
        }

        if (session.AssessmentAttempt.Score == null)
        {
            throw new ArgumentException(
                "Theory assessment score is not available.");
        }

        var theoryScore =
            session.AssessmentAttempt.Score.Value;

        var simulatorScore =
            session.Result.OverallSimulatorScore;

        // Temporary equal weighting.
        // We will make this configurable next.
        var overallScore = Math.Round(
            (theoryScore + simulatorScore) / 2,
            2);

        var result =
            overallScore >= 80
                ? "PASS"
                : "FAIL";

        return new PerformanceEvaluationResponse
        {
            SimulationSessionId = session.Id,

            ScenarioName =
                session.SimulationScenario.Name,

            DomainName =
                session.SimulationScenario
                    .SimulationDomain.Name,

            TheoryScore = theoryScore,

            SimulatorScore = simulatorScore,

            OverallScore = overallScore,

            Result = result,

            TotalViolations =
                session.Result.TotalViolations,

            CompletionTimeSeconds =
                session.Result.CompletionTimeSeconds,

            Breakdown =
                new PerformanceBreakdownResponse
                {
                    SafetyScore =
                        session.Result.SafetyScore,

                    VehicleControlScore =
                        session.Result
                            .VehicleControlScore,

                    EmergencyResponseScore =
                        session.Result
                            .EmergencyResponseScore
                },

            Events = session.Events
                .OrderBy(e => e.OccurredAtSeconds)
                .Select(e =>
                    new PerformanceEventResponse
                    {
                        EventType = e.EventType,
                        Severity = e.Severity,
                        Description = e.Description,
                        OccurredAtSeconds =
                            e.OccurredAtSeconds,
                        PenaltyPoints =
                            e.PenaltyPoints
                    })
                .ToList()
        };
    }
}