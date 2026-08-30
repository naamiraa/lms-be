namespace be.DTOs.SimulationSession;

public class CompleteSimulationRequest
{
    public double OverallSimulatorScore { get; set; }

    public double? SafetyScore { get; set; }

    public double? VehicleControlScore { get; set; }

    public double? EmergencyResponseScore { get; set; }

    public int? CompletionTimeSeconds { get; set; }

    public List<SimulationEventRequest> Events { get; set; }
        = new();
}