using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Projekt_2.Context;
using Projekt_2.Models;
using Projekt_2.ViewModels;

namespace Projekt_2.Controllers
{
    public class QuestionController(AppDbContext context) : Controller
    {
        // GET: Question
        public async Task<IActionResult> Index(string? projekt)
        {
            if (!string.IsNullOrEmpty(projekt))
            {
                var questionsByName = context.Questions.Include(q => q.Project).Where(q => q.Project.Name == projekt);
                return View(await questionsByName.Select(q => new QuestionIndexViewModel(q)).ToListAsync());
            }

            var questions = context.Questions.Include(q => q.Project);

            return View(await questions.Select(q => new QuestionIndexViewModel(q)).ToListAsync());
        }

        // GET: Question/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var question = await context.Questions
                .Include(q => q.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (question == null)
            {
                return NotFound();
            }

            var questionViewModel = new QuestionDetailsViewModel(question, question.Project);

            var testAnswers = new List<TestAnswerViewModel>(4);

            switch (question)
            {
                case TestOneQuestion or TestMultiQuestion:
                {
                    var testAnswersDb = context
                        .TestAnswers
                        .Where(ta => ta.TestQuestionId == question.Id)
                        .OrderBy(ta => ta.Numeration)
                        .ToList();
                    testAnswers
                        .AddRange(testAnswersDb
                            .Select(ta => new TestAnswerViewModel
                                {
                                    Id = ta.Id,
                                    Text = ta.Text,
                                    IsCorrect = ta.IsCorrect,
                                    Numeration = ta.Numeration
                                }
                            )
                        );
                    questionViewModel.Answers = testAnswers;
                    break;
                }
                case OpenQuestion oq:
                    questionViewModel.OpenQuestionAnswer = oq.Answer;
                    break;
            }

            return View(questionViewModel);
        }

        // GET: Question/Create
        public IActionResult Create(string type)
        {
            if (!Enum.TryParse(type, out QuestionType questionType))
            {
                return NotFound("Unknown question type");
            }

            ViewData["ProjectId"] = new SelectList(context.Projects, "Id", "Name");
            var model = new CreateQuestionViewModel
            {
                QuestionType = questionType,
            };
            return View(model);
        }

        // POST: Question/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateQuestionViewModel question)
        {
            switch (question.QuestionType)
            {
                case QuestionType.Open:
                {
                    if (string.IsNullOrWhiteSpace(question.OpenQuestionAnswer))
                    {
                        ModelState.AddModelError("OpenQuestionAnswer", "Answer is required for open questions");
                    }

                    for (var i = 0; i < question.Answers.Count; i++)
                    {
                        ModelState.Remove($"Answers[{i}].Text");
                    }

                    break;
                }
                case QuestionType.TestOne:
                case QuestionType.TestMulti:
                {
                    ModelState.Remove("OpenQuestionAnswer");

                    for (var i = 0; i < question.Answers.Count; i++)
                    {
                        if (string.IsNullOrWhiteSpace(question.Answers[i].Text))
                        {
                            ModelState.AddModelError($"Answers[{i}].Text", $"Answer {i + 1} text is required");
                        }
                    }

                    if (question.QuestionType == QuestionType.TestMulti && !question.Answers.Any(a => a.IsCorrect))
                    {
                        ModelState.AddModelError("", "At least one answer must be marked as correct");
                    }

                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException(question.QuestionType.ToString());
            }

            var answers = new List<TestAnswer>(4);

            if (question.QuestionType is QuestionType.TestOne or QuestionType.TestMulti)
            {
                for (var i = 0; i < 4; i++)
                {
                    if (question.QuestionType is QuestionType.TestOne)
                    {
                        answers.Add(new TestAnswer
                        {
                            Id = Guid.NewGuid(),
                            Text = question.Answers[i].Text,
                            IsCorrect = i == question.CorrectAnswerIndex,
                            Numeration = question.Answers[i].Numeration,
                        });
                    }
                    else
                    {
                        answers.Add(new TestAnswer
                        {
                            Id = Guid.NewGuid(),
                            Text = question.Answers[i].Text,
                            IsCorrect = question.Answers[i].IsCorrect,
                            Numeration = question.Answers[i].Numeration,
                        });
                    }
                }
            }

            Question questionDb = question.QuestionType switch
            {
                QuestionType.TestOne => new TestOneQuestion
                {
                    Id = Guid.NewGuid(),
                    QuestionText = question.QuestionText,
                    TestAnswers = answers,
                    ProjectId = question.ProjectId,
                },
                QuestionType.TestMulti => new TestMultiQuestion
                {
                    Id = Guid.NewGuid(),
                    QuestionText = question.QuestionText,
                    TestAnswers = answers,
                    ProjectId = question.ProjectId,
                },
                QuestionType.Open => new OpenQuestion
                {
                    Id = Guid.NewGuid(),
                    QuestionText = question.QuestionText,
                    Answer = question.OpenQuestionAnswer,
                    ProjectId = question.ProjectId,
                },
                _ => throw new ArgumentOutOfRangeException(nameof(question))
            };
            context.Add(questionDb);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Question/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var question = await context.Questions.FindAsync(id);
            if (question == null)
            {
                return NotFound();
            }

            var questionModel = new QuestionViewModel
            {
                Id = id.Value,
                QuestionText = question.QuestionText,
                ProjectId = question.ProjectId,
                IsOpen = false,
                OpenAnswer = string.Empty,
            };

            if (question is OpenQuestion openQuestion)
            {
                questionModel.IsOpen = true;
                questionModel.OpenAnswer = openQuestion.Answer;
            }
            ViewData["ProjectId"] = new SelectList(context.Projects, "Id", "Name", question.ProjectId);
            return View(questionModel);
        }

        // POST: Question/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,QuestionText,ProjectId,IsOpen,OpenAnswer")] QuestionViewModel question)
        {
            if (id != question.Id)
            {
                return NotFound();
            }

            if (!question.IsOpen)
            {
                ModelState.Remove("OpenAnswer");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingQuestion = await context.Questions.FindAsync(question.Id);

                    if (existingQuestion == null)
                    {
                        return NotFound();
                    }

                    existingQuestion.QuestionText = question.QuestionText;
                    existingQuestion.ProjectId = question.ProjectId;

                    if (existingQuestion is OpenQuestion openQuestion && question.IsOpen)
                    {
                        openQuestion.Answer = question.OpenAnswer;
                    }

                    await context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuestionExists(question.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
            }

            ViewData["ProjectId"] = new SelectList(context.Projects, "Id", "Name", question.ProjectId);
            return View(question);
        }

        // GET: Question/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var question = await context.Questions
                .Include(q => q.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (question == null)
            {
                return NotFound();
            }

            return View(question);
        }

        // POST: Question/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var question = await context.Questions.FindAsync(id);
            if (question != null)
            {
                context.Questions.Remove(question);
            }

            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool QuestionExists(Guid id)
        {
            return context.Questions.Any(e => e.Id == id);
        }
    }
}