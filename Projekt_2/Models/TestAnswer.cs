namespace Projekt_2.Models;

public class TestAnswer
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public Guid TestQuestionId { get; set; }
    public TestQuestion TestQuestion { get; set; } = null!;

    public TestAnswerNumeration Numeration { get; set; }
}