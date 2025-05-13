using Microsoft.EntityFrameworkCore;
using Projekt_2.Models;

namespace Projekt_2.Context;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Project> Projects { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<TestAnswer> TestAnswers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Question>()
            .HasDiscriminator<string>("QuestionType")
            .HasValue<Question>("Question")
            .HasValue<TestQuestion>("TestQuestion")
            .HasValue<OpenQuestion>("OpenQuestion")
            .HasValue<TestOneQuestion>("TestOneQuestion")
            .HasValue<TestMultiQuestion>("TestMultiQuestion");

        modelBuilder.Entity<TestAnswer>()
            .HasIndex(ta => ta.TestQuestionId);
    }
}