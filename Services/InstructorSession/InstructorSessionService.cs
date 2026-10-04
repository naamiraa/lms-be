using be.Data;
using be.DTOs.InstructorSession;
using be.Services.Evaluation;
using Microsoft.EntityFrameworkCore;

namespace be.Services.InstructorSession;

public class InstructorSessionService : IInstructorSessionService
{
    private readonly AppDbContext _context;
    private readonly IEvaluationCalculator _evaluationCalculator;

    public InstructorSessionService(
        AppDbContext context,
        IEvaluationCalculator evaluationCalculator)
    {
        _context = context;
        _evaluationCalculator = evaluationCalculator;
    }

    public async Task<List<InstructorSessionResponse>>
        GetCompletedSessionsAsync()
    {
        var sessions = await _context.SimulationSessions
            .Where(s =>
                s.Status == "COMPLETED" &&
                s.Result != null)
            .Include(s => s.User)
            .Include(s => s.Result)
            .Include(s => s.AssessmentAttempt)
            .Include(s => s.SimulationScenario)
            .Include(s => s.InstructorFeedback)
            .OrderByDescending(s => s.Result!.CreatedAt)
            .ToListAsync();

        return sessions
            .Select(s =>
            {
                var theoryScore =
                    s.AssessmentAttempt.Score ?? 0;

                var simulatorScore =
                    s.Result!.OverallSimulatorScore;

                var overallScore =
                    _evaluationCalculator.CalculateOverallScore(
                        theoryScore,
                        simulatorScore);

                return new InstructorSessionResponse
                {
                    SimulationSessionId = s.Id,

                    ParticipantName = s.User.Name,

                    ScenarioName = s.SimulationScenario.Name,

                    TheoryScore = theoryScore,

                    SimulatorScore = simulatorScore,

                    OverallScore = overallScore,

                    Result =
                        _evaluationCalculator.DetermineResult(
                            overallScore),

                    CompletedAt = s.Result.CreatedAt,

                    HasInstructorFeedback =
                        s.InstructorFeedback != null
                };
            })
            .ToList();
    }
}