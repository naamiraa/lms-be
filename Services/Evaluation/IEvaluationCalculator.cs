namespace be.Services.Evaluation;

public interface IEvaluationCalculator
{
    double CalculateOverallScore(
        double theoryScore,
        double simulatorScore);

    string DetermineResult(double overallScore);
}