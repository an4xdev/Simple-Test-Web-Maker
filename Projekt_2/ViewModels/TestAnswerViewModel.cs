using System.ComponentModel.DataAnnotations;
using Projekt_2.Models;

namespace Projekt_2.ViewModels;

public class TestAnswerViewModel
{
    public Guid Id { get;set; }
    [Required]
    public string Text { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public Guid TestQuestionId { get; set; }
    public TestAnswerNumeration Numeration { get; set; }
}