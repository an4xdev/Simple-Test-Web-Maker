namespace Projekt_2.Models;

public class Question
{
    public Guid Id { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;
}