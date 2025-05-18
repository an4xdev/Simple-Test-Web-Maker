namespace Projekt_2.ViewModels;

public class StatisticViewModel
{
    public Guid ProjectId { get; set; }

    public string ProjectName { get; set; } = string.Empty;

    public int CountOfQuestions { get; set; }

    public int CountOfTestOneQuestions { get; set; }

    public int CountOfTestMultiQuestions { get; set; }

    public int CountOfOpenQuestions { get; set; }

    public int CountOfCorrectAAnswers { get; set; }

    public int CountOfCorrectBAnswers { get; set; }

    public int CountOfCorrectCAnswers { get; set; }

    public int CountOfCorrectDAnswers { get; set; }
}