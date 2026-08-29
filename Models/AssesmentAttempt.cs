namespace be.Models;

public class AssessmentAttempt
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid AssessmentId { get; set; }
    public Assessment Assessment { get; set; } = null!;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public int AttemptNumber { get; set; }

    public string Status { get; set; } = "IN_PROGRESS";

    public DateTime StartedAt { get; set; } = DateTime.UtcNow;

    public DateTime? SubmittedAt { get; set; }

    public double? Score { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<AssessmentAnswer> Answers { get; set; }
        = new List<AssessmentAnswer>();

    public ICollection<SimulationSession> SimulationSessions { get; set; }
    = new List<SimulationSession>();
}