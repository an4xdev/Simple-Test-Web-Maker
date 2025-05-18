namespace Projekt_2.ViewModels;

public class QuestionViewModel
{
    public Guid Id { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public Guid ProjectId { get; set; }

    public bool IsOpen { get; set; }

    public string OpenAnswer { get; set; } = string.Empty;
}