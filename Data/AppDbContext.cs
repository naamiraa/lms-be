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
    }
}