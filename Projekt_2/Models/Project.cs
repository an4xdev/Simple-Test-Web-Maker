namespace Projekt_2.Models;

public class Project
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<Question> Questions { get; set; } = [];
}