using Microsoft.EntityFrameworkCore;
using Projekt_2.Models;

namespace Projekt_2.Context
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var context = new AppDbContext(serviceProvider.GetRequiredService<DbContextOptions<AppDbContext>>());

            if (context.Projects.Any())
            {
                return;
            }

            var transaction = context.Database.BeginTransaction();
            try
            {
                var projects = new List<Project>
                {
                    new() { Id = Guid.NewGuid(), Name = "Programming Fundamentals" },
                    new() { Id = Guid.NewGuid(), Name = "Web Development" },
                    new() { Id = Guid.NewGuid(), Name = "Data Science Basics" }
                };

                context.Projects.AddRange(projects);
                context.SaveChanges();

                AddQuestionsToProject(context, projects[0].Id, "Programming");

                AddQuestionsToProject(context, projects[1].Id, "Web");

                AddQuestionsToProject(context, projects[2].Id, "DataScience");

                context.SaveChanges();

                transaction.Commit();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                transaction.Rollback();
            }
        }

        private static void AddQuestionsToProject(AppDbContext context, Guid projectId, string domainPrefix)
        {
            for (var i = 1; i <= 4; i++)
            {
                var openQuestion = new OpenQuestion
                {
                    Id = Guid.NewGuid(),
                    ProjectId = projectId,
                    QuestionText = $"{domainPrefix} Open Question {i}",
                    Answer = $"Sample answer for {domainPrefix} Open Question {i}"
                };
                context.Questions.Add(openQuestion);
            }

            for (var i = 1; i <= 3; i++)
            {
                var testOneQuestion = new TestOneQuestion
                {
                    Id = Guid.NewGuid(),
                    ProjectId = projectId,
                    QuestionText = $"{domainPrefix} Single Choice Question {i}"
                };
                context.Questions.Add(testOneQuestion);

                var correctIndex = new Random().Next(0, 4);
                for (var j = 0; j < 4; j++)
                {
                    var answer = new TestAnswer
                    {
                        Id = Guid.NewGuid(),
                        TestQuestionId = testOneQuestion.Id,
                        Text = $"Answer {j + 1} for {domainPrefix} Single Choice Question {i}",
                        IsCorrect = j == correctIndex,
                        Numeration = (TestAnswerNumeration)j
                    };
                    context.TestAnswers.Add(answer);
                }
            }

            for (var i = 1; i <= 3; i++)
            {
                var testMultiQuestion = new TestMultiQuestion
                {
                    Id = Guid.NewGuid(),
                    ProjectId = projectId,
                    QuestionText = $"{domainPrefix} Multiple Choice Question {i}"
                };
                context.Questions.Add(testMultiQuestion);

                var correctIndices = new[] { new Random().Next(0, 2), new Random().Next(2, 4) };
                for (var j = 0; j < 4; j++)
                {
                    var answer = new TestAnswer
                    {
                        Id = Guid.NewGuid(),
                        TestQuestionId = testMultiQuestion.Id,
                        Text = $"Answer {j + 1} for {domainPrefix} Multiple Choice Question {i}",
                        IsCorrect = correctIndices.Contains(j),
                        Numeration = (TestAnswerNumeration)j
                    };
                    context.TestAnswers.Add(answer);
                }
            }
        }
    }
}