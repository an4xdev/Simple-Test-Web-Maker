using Projekt_2.Models;

namespace Projekt_2.ViewModels;

public class QuestionIndexViewModel : Question
{
    public QuestionIndexViewModel(Question q)
    {
        Id = q.Id;
        ProjectId = q.ProjectId;
        Project = q.Project;
        QuestionText = q.QuestionText;
        QuestionType  = q switch
        {
            TestOneQuestion => QuestionType.TestOne,
            TestMultiQuestion => QuestionType.TestMulti,
            _ => QuestionType.Open
        };
        QuestionTypeStr = q switch
        {
            TestOneQuestion => "Test question with one answer",
            TestMultiQuestion => "Test question with multiple answers",
            _ => "Open question"
        };
    }
    public QuestionType QuestionType { get; init; }

    public string QuestionTypeStr { get; init; }

}