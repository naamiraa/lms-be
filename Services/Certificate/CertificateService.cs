using be.Data;
using be.DTOs.Certificate;
using be.Services.Evaluation;
using Microsoft.EntityFrameworkCore;

namespace be.Services.Certificate;

public class CertificateService : ICertificateService
{
    private readonly AppDbContext _context;
    private readonly IEvaluationCalculator _evaluationCalculator;

    public CertificateService(
        AppDbContext context,
        IEvaluationCalculator evaluationCalculator)
    {
        _context = context;
        _evaluationCalculator = evaluationCalculator;
    }

    public async Task<CertificateResponse> IssueAsync(
        Guid userId,
        Guid simulationSessionId)
    {
        var session = await GetSessionAsync(
            userId,
            simulationSessionId);

        if (session == null)
        {
            throw new ArgumentException(
                "Completed simulation session not found.");
        }

        if (session.Result == null ||
            session.AssessmentAttempt.Score == null)
        {
            throw new ArgumentException(
                "Simulation result or theory score is not available.");
        }

        var theoryScore =
            session.AssessmentAttempt.Score.Value;

        var simulatorScore =
            session.Result.OverallSimulatorScore;

        var overallScore =
            _evaluationCalculator.CalculateOverallScore(
                theoryScore,
                simulatorScore);

        var result =
            _evaluationCalculator.DetermineResult(
                overallScore);

        if (result != "PASS")
        {
            throw new InvalidOperationException(
                "Certificate is only available for participants who passed the training.");
        }

        var existing = await _context.Certificates
            .FirstOrDefaultAsync(c =>
                c.SimulationSessionId == simulationSessionId);

        if (existing != null)
        {
            return await BuildResponseAsync(
                existing.Id,
                overallScore,
                result);
        }

        var certificate = new Models.Certificate
        {
            UserId = userId,
            SimulationSessionId = simulationSessionId,
            CertificateNumber = GenerateCertificateNumber()
        };

        _context.Certificates.Add(certificate);

        await _context.SaveChangesAsync();

        return await BuildResponseAsync(
            certificate.Id,
            overallScore,
            result);
    }

    public async Task<CertificateResponse?> GetBySessionAsync(
        Guid userId,
        Guid simulationSessionId)
    {
        var certificate = await _context.Certificates
            .FirstOrDefaultAsync(c =>
                c.UserId == userId &&
                c.SimulationSessionId == simulationSessionId);

        if (certificate == null)
        {
            return null;
        }

        var session = await GetSessionAsync(
            userId,
            simulationSessionId);

        if (session?.Result == null ||
            session.AssessmentAttempt.Score == null)
        {
            return null;
        }

        var overallScore =
            _evaluationCalculator.CalculateOverallScore(
                session.AssessmentAttempt.Score.Value,
                session.Result.OverallSimulatorScore);

        var result =
            _evaluationCalculator.DetermineResult(
                overallScore);

        return await BuildResponseAsync(
            certificate.Id,
            overallScore,
            result);
    }

    public async Task<List<CertificateResponse>>
        GetMyCertificatesAsync(Guid userId)
    {
        var certificates = await _context.Certificates
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.IssuedAt)
            .Select(c => c.Id)
            .ToListAsync();

        var results = new List<CertificateResponse>();

        foreach (var certificateId in certificates)
        {
            var certificate = await _context.Certificates
                .FirstAsync(c => c.Id == certificateId);

            var session = await GetSessionAsync(
                userId,
                certificate.SimulationSessionId);

            if (session?.Result == null ||
                session.AssessmentAttempt.Score == null)
            {
                continue;
            }

            var overallScore =
                _evaluationCalculator.CalculateOverallScore(
                    session.AssessmentAttempt.Score.Value,
                    session.Result.OverallSimulatorScore);

            var result =
                _evaluationCalculator.DetermineResult(
                    overallScore);

            results.Add(
                await BuildResponseAsync(
                    certificate.Id,
                    overallScore,
                    result));
        }

        return results;
    }

    private async Task<Models.SimulationSession?>
        GetSessionAsync(
            Guid userId,
            Guid simulationSessionId)
    {
        return await _context.SimulationSessions
            .Include(s => s.User)
            .Include(s => s.Result)
            .Include(s => s.SimulationScenario)
            .Include(s => s.AssessmentAttempt)
                .ThenInclude(a => a.Assessment)
                    .ThenInclude(a => a.LearningModule)
            .FirstOrDefaultAsync(s =>
                s.Id == simulationSessionId &&
                s.UserId == userId &&
                s.Status == "COMPLETED");
    }

    private async Task<CertificateResponse>
        BuildResponseAsync(
            Guid certificateId,
            double overallScore,
            string result)
    {
        var certificate = await _context.Certificates
            .Include(c => c.User)
            .Include(c => c.SimulationSession)
                .ThenInclude(s => s.SimulationScenario)
            .Include(c => c.SimulationSession)
                .ThenInclude(s => s.AssessmentAttempt)
                    .ThenInclude(a => a.Assessment)
                        .ThenInclude(a => a.LearningModule)
            .FirstAsync(c => c.Id == certificateId);

        return new CertificateResponse
        {
            Id = certificate.Id,
            CertificateNumber =
                certificate.CertificateNumber,

            SimulationSessionId =
                certificate.SimulationSessionId,

            ParticipantName =
                certificate.User.Name,

            LearningModuleName =
                certificate.SimulationSession
                    .AssessmentAttempt
                    .Assessment
                    .LearningModule
                    .Name,

            ScenarioName =
                certificate.SimulationSession
                    .SimulationScenario
                    .Name,

            OverallScore = overallScore,
            Result = result,
            IssuedAt = certificate.IssuedAt
        };
    }

    private static string GenerateCertificateNumber()
    {
        return $"CERT-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
    }
}
