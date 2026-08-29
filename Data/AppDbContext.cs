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
    }
}