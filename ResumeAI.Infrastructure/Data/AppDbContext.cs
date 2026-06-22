using Microsoft.EntityFrameworkCore;
using ResumeAI.Core.Entities;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace ResumeAI.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Resume> Resumes => Set<Resume>();
    public DbSet<JobDescription> JobDescriptions => Set<JobDescription>();
    public DbSet<AnalysisResult> AnalysisResults => Set<AnalysisResult>();
    public DbSet<SkillMatch> SkillMatches => Set<SkillMatch>();
    public DbSet<InterviewSession> InterviewSessions => Set<InterviewSession>();
    public DbSet<InterviewQuestion> InterviewQuestions => Set<InterviewQuestion>();
    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(e =>
        {
            e.HasIndex(u => u.Email).IsUnique();
            e.Property(u => u.Email).HasMaxLength(256).IsRequired();
            e.Property(u => u.FullName).HasMaxLength(200).IsRequired();
            e.Property(u => u.Role).HasMaxLength(50);
        });

        modelBuilder.Entity<Resume>(e =>
        {
            e.HasOne(r => r.User).WithMany(u => u.Resumes)
                .HasForeignKey(r => r.UserId).OnDelete(DeleteBehavior.Cascade);
            e.Property(r => r.FileName).HasMaxLength(300);
            e.Property(r => r.FilePath).HasMaxLength(500);
        });

        modelBuilder.Entity<JobDescription>(e =>
        {
            e.HasOne(j => j.User).WithMany(u => u.JobDescriptions)
                .HasForeignKey(j => j.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<AnalysisResult>(e =>
        {
            e.HasOne(a => a.Resume).WithMany(r => r.AnalysisResults)
                .HasForeignKey(a => a.ResumeId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(a => a.JobDescription).WithMany(j => j.AnalysisResults)
                .HasForeignKey(a => a.JobDescriptionId).OnDelete(DeleteBehavior.SetNull);
            e.Property(a => a.OverallScore).HasColumnType("decimal(5,2)");
            e.Property(a => a.SkillScore).HasColumnType("decimal(5,2)");
            e.Property(a => a.ExperienceScore).HasColumnType("decimal(5,2)");
            e.Property(a => a.FormatScore).HasColumnType("decimal(5,2)");
        });

        modelBuilder.Entity<SkillMatch>(e =>
        {
            e.HasOne(s => s.Analysis).WithMany(a => a.SkillMatches)
                .HasForeignKey(s => s.AnalysisId).OnDelete(DeleteBehavior.Cascade);
            e.Property(s => s.MatchScore).HasColumnType("decimal(5,2)");
        });

        modelBuilder.Entity<InterviewSession>(e =>
        {
            e.HasOne(s => s.User).WithMany(u => u.InterviewSessions)
                .HasForeignKey(s => s.UserId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(s => s.Resume).WithMany(r => r.InterviewSessions)
                .HasForeignKey(s => s.ResumeId).OnDelete(DeleteBehavior.Cascade);
            e.Property(s => s.TotalScore).HasColumnType("decimal(5,2)");
        });

        modelBuilder.Entity<InterviewQuestion>(e =>
        {
            e.HasOne(q => q.Session).WithMany(s => s.Questions)
                .HasForeignKey(q => q.SessionId).OnDelete(DeleteBehavior.Cascade);
            e.Property(q => q.Score).HasColumnType("decimal(5,2)");
        });

        modelBuilder.Entity<Notification>(e =>
        {
            e.HasOne(n => n.User).WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId).OnDelete(DeleteBehavior.Cascade);
        });
    }
}