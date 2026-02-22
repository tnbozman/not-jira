using Microsoft.EntityFrameworkCore;
using StoryFirst.Api.Models;

namespace StoryFirst.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<TaskItem> Tasks { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectMember> ProjectMembers { get; set; }
    public DbSet<ExternalEntity> ExternalEntities { get; set; }
    public DbSet<Problem> Problems { get; set; }
    public DbSet<Outcome> Outcomes { get; set; }
    public DbSet<SuccessMetric> SuccessMetrics { get; set; }
    public DbSet<Interview> Interviews { get; set; }
    public DbSet<InterviewNote> InterviewNotes { get; set; }
    public DbSet<Tag> Tags { get; set; }
    
    // User Story Map entities
    public DbSet<Theme> Themes { get; set; }
    public DbSet<Epic> Epics { get; set; }
    public DbSet<Story> Stories { get; set; }
    public DbSet<Spike> Spikes { get; set; }
    public DbSet<Sprint> Sprints { get; set; }
    public DbSet<Team> Teams { get; set; }
    public DbSet<Release> Releases { get; set; }
    public DbSet<TeamPlanning> TeamPlannings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Priority).IsRequired().HasMaxLength(50);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Key).IsRequired().HasMaxLength(20);
            entity.HasIndex(e => e.Key).IsUnique();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            
            entity.HasMany(e => e.Members)
                .WithOne(e => e.Project)
                .HasForeignKey(e => e.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ProjectMember>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserId).IsRequired().HasMaxLength(100);
            entity.Property(e => e.UserName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.UserEmail).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Role).IsRequired();
            entity.Property(e => e.AddedAt).IsRequired();
            
            // Ensure a user can only have one role per project
            entity.HasIndex(e => new { e.ProjectId, e.UserId }).IsUnique();
        });

        // ExternalEntity
        modelBuilder.Entity<ExternalEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Type).IsRequired();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.Organization).HasMaxLength(200);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.Notes).HasMaxLength(2000);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            
            entity.HasOne(e => e.Project)
                .WithMany()
                .HasForeignKey(e => e.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Problem
        modelBuilder.Entity<Problem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.Severity).IsRequired();
            entity.Property(e => e.Context).HasMaxLength(2000);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            
            entity.HasOne(e => e.ExternalEntity)
                .WithMany(e => e.Problems)
                .HasForeignKey(e => e.ExternalEntityId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Outcome
        modelBuilder.Entity<Outcome>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.Priority).IsRequired();
            entity.Property(e => e.Context).HasMaxLength(2000);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            
            entity.HasOne(e => e.ExternalEntity)
                .WithMany()
                .HasForeignKey(e => e.ExternalEntityId)
                .OnDelete(DeleteBehavior.Cascade);
                
            // Many-to-many with Problems
            entity.HasMany(e => e.Problems)
                .WithMany(p => p.Outcomes);
        });

        // SuccessMetric
        modelBuilder.Entity<SuccessMetric>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(500);
            entity.Property(e => e.TargetValue).HasMaxLength(100);
            entity.Property(e => e.CurrentValue).HasMaxLength(100);
            entity.Property(e => e.Unit).HasMaxLength(50);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            
            entity.HasOne(e => e.Outcome)
                .WithMany(o => o.SuccessMetrics)
                .HasForeignKey(e => e.OutcomeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Interview
        modelBuilder.Entity<Interview>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Type).IsRequired();
            entity.Property(e => e.InterviewDate).IsRequired();
            entity.Property(e => e.Interviewer).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Summary).HasMaxLength(2000);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            
            entity.HasOne(e => e.ExternalEntity)
                .WithMany(e => e.Interviews)
                .HasForeignKey(e => e.ExternalEntityId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(e => e.Project)
                .WithMany()
                .HasForeignKey(e => e.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // InterviewNote
        modelBuilder.Entity<InterviewNote>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Content).IsRequired().HasMaxLength(2000);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            
            entity.HasOne(e => e.Interview)
                .WithMany(i => i.Notes)
                .HasForeignKey(e => e.InterviewId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(e => e.RelatedProblem)
                .WithMany()
                .HasForeignKey(e => e.RelatedProblemId)
                .OnDelete(DeleteBehavior.SetNull);
                
            entity.HasOne(e => e.RelatedOutcome)
                .WithMany()
                .HasForeignKey(e => e.RelatedOutcomeId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Tag
        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).IsRequired();
            
            entity.HasOne(e => e.Project)
                .WithMany()
                .HasForeignKey(e => e.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
                
            // Ensure tag names are unique per project
            entity.HasIndex(e => new { e.ProjectId, e.Name }).IsUnique();
        });

        // EntityTag junction table
        modelBuilder.Entity<EntityTag>(entity =>
        {
            entity.HasKey(e => new { e.ExternalEntityId, e.TagId });
            
            entity.HasOne(e => e.ExternalEntity)
                .WithMany(e => e.EntityTags)
                .HasForeignKey(e => e.ExternalEntityId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(e => e.Tag)
                .WithMany(t => t.EntityTags)
                .HasForeignKey(e => e.TagId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ProblemTag junction table
        modelBuilder.Entity<ProblemTag>(entity =>
        {
            entity.HasKey(e => new { e.ProblemId, e.TagId });
            
            entity.HasOne(e => e.Problem)
                .WithMany(p => p.ProblemTags)
                .HasForeignKey(e => e.ProblemId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(e => e.Tag)
                .WithMany(t => t.ProblemTags)
                .HasForeignKey(e => e.TagId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // OutcomeTag junction table
        modelBuilder.Entity<OutcomeTag>(entity =>
        {
            entity.HasKey(e => new { e.OutcomeId, e.TagId });
            
            entity.HasOne(e => e.Outcome)
                .WithMany(o => o.OutcomeTags)
                .HasForeignKey(e => e.OutcomeId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(e => e.Tag)
                .WithMany(t => t.OutcomeTags)
                .HasForeignKey(e => e.TagId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Theme
        modelBuilder.Entity<Theme>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.Order).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            
            entity.HasOne(e => e.Project)
                .WithMany()
                .HasForeignKey(e => e.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(e => e.Outcome)
                .WithMany()
                .HasForeignKey(e => e.OutcomeId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Epic
        modelBuilder.Entity<Epic>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.Order).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            
            entity.HasOne(e => e.Theme)
                .WithMany(t => t.Epics)
                .HasForeignKey(e => e.ThemeId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(e => e.Outcome)
                .WithMany()
                .HasForeignKey(e => e.OutcomeId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Story
        modelBuilder.Entity<Story>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.SolutionDescription).HasMaxLength(2000);
            entity.Property(e => e.AcceptanceCriteria).HasMaxLength(2000);
            entity.Property(e => e.Order).IsRequired();
            entity.Property(e => e.Priority).IsRequired();
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            
            entity.HasOne(e => e.Epic)
                .WithMany(ep => ep.Stories)
                .HasForeignKey(e => e.EpicId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(e => e.Sprint)
                .WithMany(s => s.Stories)
                .HasForeignKey(e => e.SprintId)
                .OnDelete(DeleteBehavior.SetNull);
                
            entity.HasOne(e => e.Team)
                .WithMany(t => t.Stories)
                .HasForeignKey(e => e.TeamId)
                .OnDelete(DeleteBehavior.SetNull);
                
            entity.HasOne(e => e.Release)
                .WithMany(r => r.Stories)
                .HasForeignKey(e => e.ReleaseId)
                .OnDelete(DeleteBehavior.SetNull);
                
            entity.HasOne(e => e.Outcome)
                .WithMany()
                .HasForeignKey(e => e.OutcomeId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Spike
        modelBuilder.Entity<Spike>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.InvestigationGoal).HasMaxLength(2000);
            entity.Property(e => e.Findings).HasMaxLength(2000);
            entity.Property(e => e.Order).IsRequired();
            entity.Property(e => e.Priority).IsRequired();
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            
            entity.HasOne(e => e.Epic)
                .WithMany(ep => ep.Spikes)
                .HasForeignKey(e => e.EpicId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(e => e.Sprint)
                .WithMany(s => s.Spikes)
                .HasForeignKey(e => e.SprintId)
                .OnDelete(DeleteBehavior.SetNull);
                
            entity.HasOne(e => e.Team)
                .WithMany(t => t.Spikes)
                .HasForeignKey(e => e.TeamId)
                .OnDelete(DeleteBehavior.SetNull);
                
            entity.HasOne(e => e.Release)
                .WithMany(r => r.Spikes)
                .HasForeignKey(e => e.ReleaseId)
                .OnDelete(DeleteBehavior.SetNull);
                
            entity.HasOne(e => e.Outcome)
                .WithMany()
                .HasForeignKey(e => e.OutcomeId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Sprint
        modelBuilder.Entity<Sprint>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Goal).HasMaxLength(1000);
            entity.Property(e => e.PlanningOneNotes).HasMaxLength(4000);
            entity.Property(e => e.ReviewNotes).HasMaxLength(4000);
            entity.Property(e => e.RetroNotes).HasMaxLength(4000);
            entity.Property(e => e.StartDate).IsRequired();
            entity.Property(e => e.EndDate).IsRequired();
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            
            entity.HasOne(e => e.Project)
                .WithMany()
                .HasForeignKey(e => e.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        // Team
        modelBuilder.Entity<Team>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            
            entity.HasOne(e => e.Project)
                .WithMany()
                .HasForeignKey(e => e.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        // Release
        modelBuilder.Entity<Release>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            
            entity.HasOne(e => e.Project)
                .WithMany()
                .HasForeignKey(e => e.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        // TeamPlanning
        modelBuilder.Entity<TeamPlanning>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PlanningTwoNotes).HasMaxLength(4000);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            
            entity.HasOne(e => e.Sprint)
                .WithMany(s => s.TeamPlannings)
                .HasForeignKey(e => e.SprintId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(e => e.Team)
                .WithMany()
                .HasForeignKey(e => e.TeamId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
