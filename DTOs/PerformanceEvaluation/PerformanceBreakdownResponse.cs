namespace be.DTOs.PerformanceEvaluation;

public class PerformanceBreakdownResponse
{
    public double? SafetyScore { get; set; }

    public double? VehicleControlScore { get; set; }

    public double? EmergencyResponseScore { get; set; }
}