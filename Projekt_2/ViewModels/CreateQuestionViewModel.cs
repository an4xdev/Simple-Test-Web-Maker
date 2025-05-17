using System.ComponentModel.DataAnnotations;
using Projekt_2.Models;

namespace Projekt_2.ViewModels;

public class CreateQuestionViewModel
{
    [Required(ErrorMessage = "Question text is required")]
    public string QuestionText { get; set; } = string.Empty;

    public Guid ProjectId { get; set; }

    public QuestionType QuestionType { get; set; }

    [RequiredIf(nameof(QuestionType), QuestionType.Open, ErrorMessage = "Answer is required for open questions")]
    public string OpenQuestionAnswer { get; set; } = string.Empty;

    public int CorrectAnswerIndex { get; set; }

    public List<TestAnswerViewModel> Answers { get; set; } =
    [
        new() { IsCorrect = false, Text = string.Empty, Numeration = TestAnswerNumeration.A },
        new() { IsCorrect = false, Text = string.Empty, Numeration = TestAnswerNumeration.B },
        new() { IsCorrect = false, Text = string.Empty, Numeration = TestAnswerNumeration.C },
        new() { IsCorrect = false, Text = string.Empty, Numeration = TestAnswerNumeration.D }
    ];
}