using be.Models;
using Microsoft.EntityFrameworkCore;

namespace be.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
   
    public DbSet<TrainingDomain> TrainingDomains => Set<TrainingDomain>();
    public DbSet<LearningModule> LearningModules => Set<LearningModule>();
    public DbSet<LearningContent> LearningContents => Set<LearningContent>();
    public DbSet<LearningProgress> LearningProgresses => Set<LearningProgress>();

    public DbSet<Assessment> Assessments => Set<Assessment>();

    public DbSet<AssessmentQuestion> AssessmentQuestions
        => Set<AssessmentQuestion>();

    public DbSet<QuestionOption> QuestionOptions
        => Set<QuestionOption>();

    public DbSet<AssessmentAttempt> AssessmentAttempts
        => Set<AssessmentAttempt>();

    public DbSet<AssessmentAnswer> AssessmentAnswers
        => Set<AssessmentAnswer>();

    public DbSet<SimulationDomain> SimulationDomains
    => Set<SimulationDomain>();

    public DbSet<SimulationScenario> SimulationScenarios
        => Set<SimulationScenario>();

    public DbSet<SimulationSession> SimulationSessions
        => Set<SimulationSession>();

    public DbSet<SimulationResult> SimulationResults
        => Set<SimulationResult>();

    public DbSet<SimulationEvent> SimulationEvents
        => Set<SimulationEvent>();

    public DbSet<InstructorFeedback> InstructorFeedbacks
        => Set<InstructorFeedback>();

    public DbSet<FeedbackStrength> FeedbackStrengths
        => Set<FeedbackStrength>();

    public DbSet<FeedbackImprovementArea> FeedbackImprovementAreas
        => Set<FeedbackImprovementArea>();

    public DbSet<SafetyRecommendation> SafetyRecommendations
        => Set<SafetyRecommendation>();

    public DbSet<ViolationExplanation> ViolationExplanations
        => Set<ViolationExplanation>();

    public DbSet<FeedbackRecommendedModule> FeedbackRecommendedModules
        => Set<FeedbackRecommendedModule>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserRole>()
            .HasKey(ur => new { ur.UserId, ur.RoleId });

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId);

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId);

        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "ADMIN" },
            new Role { Id = 2, Name = "INSTRUCTOR" },
            new Role { Id = 3, Name = "PARTICIPANT" }
        );

        modelBuilder.Entity<LearningModule>()
            .HasOne(m => m.TrainingDomain)
            .WithMany(d => d.LearningModules)
            .HasForeignKey(m => m.TrainingDomainId);

        modelBuilder.Entity<LearningContent>()
            .HasOne(c => c.LearningModule)
            .WithMany(m => m.Contents)
            .HasForeignKey(c => c.LearningModuleId);

        modelBuilder.Entity<LearningProgress>()
            .HasOne(p => p.User)
            .WithMany()
            .HasForeignKey(p => p.UserId);

        modelBuilder.Entity<LearningProgress>()
            .HasOne(p => p.LearningContent)
            .WithMany()
            .HasForeignKey(p => p.LearningContentId);

        modelBuilder.Entity<LearningProgress>()
            .HasIndex(p => new
            {
                p.UserId,
                p.LearningContentId
            })
            .IsUnique();

        modelBuilder.Entity<Assessment>()
            .HasOne(a => a.LearningModule)
            .WithMany(m => m.Assessments)
            .HasForeignKey(a => a.LearningModuleId);

        modelBuilder.Entity<AssessmentQuestion>()
            .HasOne(q => q.Assessment)
            .WithMany(a => a.Questions)
            .HasForeignKey(q => q.AssessmentId);

        modelBuilder.Entity<QuestionOption>()
            .HasOne(o => o.AssessmentQuestion)
            .WithMany(q => q.Options)
            .HasForeignKey(o => o.AssessmentQuestionId);

        modelBuilder.Entity<AssessmentAttempt>()
            .HasOne(a => a.Assessment)
            .WithMany(a => a.Attempts)
            .HasForeignKey(a => a.AssessmentId);

        modelBuilder.Entity<AssessmentAttempt>()
            .HasOne(a => a.User)
            .WithMany()
            .HasForeignKey(a => a.UserId);

        modelBuilder.Entity<AssessmentAttempt>()
            .HasIndex(a => new
            {
                a.AssessmentId,
                a.UserId,
                a.AttemptNumber
            })
            .IsUnique();

        modelBuilder.Entity<AssessmentAnswer>()
            .HasOne(a => a.AssessmentAttempt)
            .WithMany(a => a.Answers)
            .HasForeignKey(a => a.AssessmentAttemptId);

        modelBuilder.Entity<AssessmentAnswer>()
            .HasOne(a => a.AssessmentQuestion)
            .WithMany()
            .HasForeignKey(a => a.AssessmentQuestionId);

        modelBuilder.Entity<AssessmentAnswer>()
            .HasOne(a => a.SelectedOption)
            .WithMany()
            .HasForeignKey(a => a.SelectedOptionId);


        modelBuilder.Entity<AssessmentAnswer>()
            .HasIndex(a => new
            {
                a.AssessmentAttemptId,
                a.AssessmentQuestionId
            })
            .IsUnique();


        modelBuilder.Entity<SimulationScenario>()
            .HasOne(s => s.SimulationDomain)
            .WithMany(d => d.Scenarios)
            .HasForeignKey(s => s.SimulationDomainId);

        modelBuilder.Entity<SimulationSession>()
            .HasOne(s => s.User)
            .WithMany()
            .HasForeignKey(s => s.UserId);

        modelBuilder.Entity<SimulationSession>()
            .HasOne(s => s.SimulationScenario)
            .WithMany(s => s.Sessions)
            .HasForeignKey(s => s.SimulationScenarioId);

        modelBuilder.Entity<SimulationSession>()
            .HasOne(s => s.AssessmentAttempt)
            .WithMany(a => a.SimulationSessions)
            .HasForeignKey(s => s.AssessmentAttemptId);

        modelBuilder.Entity<SimulationResult>()
            .HasOne(r => r.SimulationSession)
            .WithOne(s => s.Result)
            .HasForeignKey<SimulationResult>(
                r => r.SimulationSessionId);

        modelBuilder.Entity<SimulationResult>()
            .HasIndex(r => r.SimulationSessionId)
            .IsUnique();

        modelBuilder.Entity<SimulationEvent>()
            .HasOne(e => e.SimulationSession)
            .WithMany(s => s.Events)
            .HasForeignKey(e => e.SimulationSessionId);


        modelBuilder.Entity<InstructorFeedback>()
            .HasOne(f => f.SimulationSession)
            .WithOne(s => s.InstructorFeedback)
            .HasForeignKey<InstructorFeedback>(
                f => f.SimulationSessionId);

        modelBuilder.Entity<InstructorFeedback>()
            .HasIndex(f => f.SimulationSessionId)
            .IsUnique();

        modelBuilder.Entity<InstructorFeedback>()
            .HasOne(f => f.Instructor)
            .WithMany()
            .HasForeignKey(f => f.InstructorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<FeedbackStrength>()
            .HasOne(s => s.InstructorFeedback)
            .WithMany(f => f.Strengths)
            .HasForeignKey(s => s.InstructorFeedbackId);

        modelBuilder.Entity<FeedbackImprovementArea>()
            .HasOne(a => a.InstructorFeedback)
            .WithMany(f => f.ImprovementAreas)
            .HasForeignKey(a => a.InstructorFeedbackId);

        modelBuilder.Entity<SafetyRecommendation>()
            .HasOne(r => r.InstructorFeedback)
            .WithMany(f => f.SafetyRecommendations)
            .HasForeignKey(r => r.InstructorFeedbackId);

        modelBuilder.Entity<ViolationExplanation>()
            .HasOne(v => v.InstructorFeedback)
            .WithMany(f => f.ViolationExplanations)
            .HasForeignKey(v => v.InstructorFeedbackId);

        modelBuilder.Entity<ViolationExplanation>()
            .HasOne(v => v.SimulationEvent)
            .WithMany()
            .HasForeignKey(v => v.SimulationEventId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<FeedbackRecommendedModule>()
            .HasOne(r => r.InstructorFeedback)
            .WithMany(f => f.RecommendedModules)
            .HasForeignKey(r => r.InstructorFeedbackId);

        modelBuilder.Entity<FeedbackRecommendedModule>()
            .HasOne(r => r.LearningModule)
            .WithMany()
            .HasForeignKey(r => r.LearningModuleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}