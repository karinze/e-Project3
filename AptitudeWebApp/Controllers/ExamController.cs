using AptitudeWebApp.DAL;
using AptitudeWebApp.Models;
using AptitudeWebApp.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace AptitudeWebApp.Controllers
{
    [Route("exam")]
    public class ExamController : Controller
    {
        //private readonly IGenericRepository<Exam> _db;
        private readonly AptitudeContext _context;
        ApplicantExam apExam = new ApplicantExam(); //initialize new ApplicantExam
        public ExamController(/*IGenericRepository<Exam> db*/ AptitudeContext context)
        {
            //_db = db;
            _context = context;
        }
        [Route("index")]
        [Route("")]
        [Route("~")]
        public IActionResult Index(int? examTypeId)
        {
            var model = new List<ExamQuestions>();
            Random random = new Random();

            var exam = _context.Exams
            .Include(e => e.ExamQuestions)
            .FirstOrDefault(e => e.ExamTypeId == examTypeId);

            if (exam == null)
            {
                return NotFound(); // Handle case where exam type is not found
            }

            var questions = exam.ExamQuestions.ToList();
            // Group questions by exam type
            var groupedQuestions = questions.GroupBy(q => q.ExamTypeId).ToList();

            // Shuffle questions within each group
            var shuffledQuestions = groupedQuestions.SelectMany(g => g.OrderBy(q => random.Next()).ToList());

            ViewBag.ExamTypeId = examTypeId;
            //ViewBag.ExamDurationMinutes = exam.DurationMinutes;

            return View(shuffledQuestions);
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }
        [HttpPost]
        [Route("submit")]
        public IActionResult Submit(List<int> answers)
        {
            var questions = _context.ExamQuestions.Include(q => q.Answers).ToList();
            var totalQuestions = questions.Count;
            var correctAnswers = 0;

            for (int i = 0; i < totalQuestions; i++)
            {
                var selectedAnswerId = answers[i];
                var correctAnswerId = questions[i].Answers.FirstOrDefault(a => a.IsCorrect)?.Id;

                if (selectedAnswerId == correctAnswerId)
                {
                    correctAnswers++;
                }
            }

            var score = (correctAnswers * 100) / totalQuestions;

            ViewBag.Score = score;

            return View("Result");

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
