using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Projekt_2.Context;
using Projekt_2.Models;
using Projekt_2.ViewModels;

namespace Projekt_2.Controllers
{
    public class TestAnswerController(AppDbContext context) : Controller
    {
        // GET: TestAnswer
        public async Task<IActionResult> Index(string searchQuestion, string searchAnswer, string sortOrder,
            int? pageNumber, int pageSize = 10)
        {
            ViewData["QuestionSortParam"] = string.IsNullOrEmpty(sortOrder) ? "question_desc" : "";
            ViewData["CorrectSortParam"] = sortOrder == "correct" ? "correct_desc" : "correct";
            ViewData["NumerationSortParam"] = sortOrder == "numeration" ? "numeration_desc" : "numeration";

            ViewData["CurrentQuestionFilter"] = searchQuestion;
            ViewData["CurrentAnswerFilter"] = searchAnswer;

            var answers = context.TestAnswers
                .Include(t => t.TestQuestion)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchQuestion))
            {
                answers = answers.Where(a => a.TestQuestion.QuestionText.Contains(searchQuestion));
            }

            if (!string.IsNullOrEmpty(searchAnswer))
            {
                answers = answers.Where(a => a.Text.Contains(searchAnswer));
            }

            answers = sortOrder switch
            {
                "question_desc" => answers.OrderByDescending(a => a.TestQuestion.QuestionText),
                "correct" => answers.OrderBy(a => a.IsCorrect),
                "correct_desc" => answers.OrderByDescending(a => a.IsCorrect),
                "numeration" => answers.OrderBy(a => a.Numeration),
                "numeration_desc" => answers.OrderByDescending(a => a.Numeration),
                _ => answers.OrderBy(a => a.TestQuestion.QuestionText)
            };

            var pageNumberValue = pageNumber ?? 1;
            var pagedAnswers = await PaginatedList<TestAnswer>.CreateAsync(answers, pageNumberValue, pageSize);

            return View(pagedAnswers);
        }

        // GET: TestAnswer/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var testAnswer = await context.TestAnswers
                .Include(t => t.TestQuestion)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (testAnswer == null)
            {
                return NotFound();
            }

            return View(testAnswer);
        }

        // GET: TestAnswer/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var testAnswer = await context.TestAnswers.Include(ta => ta.TestQuestion).FirstOrDefaultAsync(ta => ta.Id == id);
            if (testAnswer == null)
            {
                return NotFound();
            }

            return View(testAnswer);
        }

        // POST: TestAnswer/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Text,IsCorrect,TestQuestionId")] TestAnswer testAnswer)
        {
            if (id != testAnswer.Id)
            {
                return NotFound();
            }

            var existingAnswer = await context.TestAnswers.Include(ta => ta.TestQuestion).FirstOrDefaultAsync(ta => ta.Id == id);
            if (existingAnswer == null)
            {
                return NotFound();
            }

            if (existingAnswer.TestQuestion is TestOneQuestion && testAnswer.IsCorrect)
            {
                var anotherTestAnswer = await context.TestAnswers
                    .Where(ta => ta.TestQuestionId == existingAnswer.TestQuestionId && ta.Id != id && ta.IsCorrect)
                    .FirstOrDefaultAsync();
                // Create method should check if single answer question has exactly one answer, also on form
                // we are using radio so we have already checked one answer
                anotherTestAnswer!.IsCorrect = false;
                await context.SaveChangesAsync();
            }

            existingAnswer.Text = testAnswer.Text;
            existingAnswer.IsCorrect = testAnswer.IsCorrect;
            ModelState.Remove("TestQuestion");
            if (!ModelState.IsValid) return View(existingAnswer);
            try
            {
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TestAnswerExists(testAnswer.Id))
                {
                    return NotFound();
                }

                throw;
            }
        }

        private bool TestAnswerExists(Guid id)
        {
            return context.TestAnswers.Any(e => e.Id == id);
        }
    }
}