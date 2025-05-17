using Projekt_2.Models;

namespace Projekt_2.ViewModels;

public class QuestionDetailsViewModel : CreateQuestionViewModel
{

    public QuestionDetailsViewModel(Question q, Project project)
    {
        Project = project;
        Id = q.Id;
        QuestionText = q.QuestionText;
        QuestionType = q switch
        {
            TestOneQuestion => QuestionType.TestOne,
            TestMultiQuestion => QuestionType.TestMulti,
            _ => QuestionType.Open
        };
    }

    public Guid Id { get; set; }

    public Project Project { get; set; }
}