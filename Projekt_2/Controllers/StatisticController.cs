using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projekt_2.Context;
using Projekt_2.Models;
using Projekt_2.ViewModels;
using OpenQuestion = Projekt_2.Models.OpenQuestion;


namespace Projekt_2.Controllers;

public class StatisticController(AppDbContext context) : Controller
{
    public IActionResult Index(Guid? id)
    {
        var projects = context.Projects.AsQueryable();

        if (id != null)
        {
            projects = projects.Where(p => p.Id == id);
        }

        var projectResult = projects.ToList();

        var result = new List<StatisticViewModel>(projectResult.Count);
        result.AddRange(projectResult.Select(project => new StatisticViewModel
        {
            ProjectId = project.Id,
            ProjectName = project.Name,

            CountOfQuestions = context
                .Questions
                .Count(q => q.ProjectId == project.Id),

            CountOfTestOneQuestions = context
                .Questions
                .OfType<TestOneQuestion>()
                .Count(q => q.ProjectId == project.Id),

            CountOfTestMultiQuestions = context.Questions
                .OfType<TestMultiQuestion>()
                .Count(q => q.ProjectId == project.Id),

            CountOfOpenQuestions = context.Questions
                .OfType<OpenQuestion>()
                .Count(q => q.ProjectId == project.Id),

            CountOfCorrectAAnswers = context
                .TestAnswers
                .Include(ta => ta.TestQuestion)
                .Count(ta => ta.TestQuestion.ProjectId == project.Id && ta.IsCorrect && ta.Numeration == TestAnswerNumeration.A),

            CountOfCorrectBAnswers = context
                .TestAnswers
                .Include(ta => ta.TestQuestion)
                .Count(ta => ta.TestQuestion.ProjectId == project.Id && ta.IsCorrect && ta.Numeration == TestAnswerNumeration.B),

            CountOfCorrectCAnswers = context
                .TestAnswers
                .Include(ta => ta.TestQuestion)
                .Count(ta => ta.TestQuestion.ProjectId == project.Id && ta.IsCorrect && ta.Numeration == TestAnswerNumeration.C),

            CountOfCorrectDAnswers = context
                .TestAnswers
                .Include(ta => ta.TestQuestion)
                .Count(ta => ta.TestQuestion.ProjectId == project.Id && ta.IsCorrect && ta.Numeration == TestAnswerNumeration.D),
        }));

        return View(result);
    }
}