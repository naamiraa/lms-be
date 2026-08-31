using be.Settings;
using Microsoft.Extensions.Options;

namespace be.Services.Evaluation;

public class EvaluationCalculator : IEvaluationCalculator
{
    private readonly EvaluationSettings _settings;

    public EvaluationCalculator(
        IOptions<EvaluationSettings> options)
    {
        _settings = options.Value;
    }

    public double CalculateOverallScore(
        double theoryScore,
        double simulatorScore)
    {
        var totalWeight =
            _settings.TheoryWeight +
            _settings.SimulatorWeight;

        if (Math.Abs(totalWeight - 1.0) > 0.0001)
        {
            throw new InvalidOperationException(
                "Evaluation weights must total 1.0.");
        }

        var score =
            (theoryScore * _settings.TheoryWeight) +
            (simulatorScore * _settings.SimulatorWeight);

        return Math.Round(score, 2);
    }

    public string DetermineResult(double overallScore)
    {
        return overallScore >= _settings.PassingScore
            ? "PASS"
            : "FAIL";
    }
}
