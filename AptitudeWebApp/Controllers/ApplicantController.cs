using AptitudeWebApp.DAL;
using AptitudeWebApp.Models;
using AptitudeWebApp.Models.Authentication;
using AptitudeWebApp.Repository;
using AptitudeWebApp.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace AptitudeWebApp.Controllers
{
    public class ApplicantController : Controller
    {
        private readonly AptitudeContext _context;
        private readonly IExamService _examService;

        public ApplicantController(AptitudeContext context, IExamService examService)
        {
            _context = context;
            _examService = examService;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult ExamDashboard()
        {
            // Check if Applicant has logged in
            if (HttpContext.Session.GetString("Applicant") == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }
        [HttpGet]
        [Route("exam/start/{applicantId}/{examTypeId:int}")]
        public IActionResult StartExam(string applicantId, int examTypeId)
        {
            var applicantGuid = Guid.Parse(applicantId);
            if (HttpContext.Session.GetString("Applicant") == null)
            {
                return RedirectToAction("Login", "Account");
            }
            // Get 5 random questions with the exam type along with their answers
            var questions = _examService.GetRandomQuestionsByExamType(examTypeId, 5);

            // Initialize the Exam parameters
            var exam = _examService.InitializeExam(applicantGuid, questions, examTypeId);

            exam.ExamQuestions.AddRange(questions);
            return View("Exam", exam);
        }

        [HttpPost]
        public IActionResult SubmitAnswer(string applicantId, int examId, int examTypeId)
        {
            Exam exam = _context.Exams.FirstOrDefault(x => x.ExamId == examId);
            var applicantGuid = Guid.Parse(applicantId);

            if (exam != null)
            {
                int score = _examService.ProcessAnswers(exam);

                _examService.SaveExamScore(exam, score);

                var applicant = _examService.GetApplicantByApplicantId(applicantGuid);

                // Check if all three exam types are completed
                if (applicant.CompletedExamTypes.Count == 3)
                {
                    // Display the result page with the total score
                    return RedirectToAction("Result", new { applicantId = applicant.ApplicantId.ToString() });
                }
                // Redirect to the next exam type if not done
                else if (applicant.CompletedExamTypes.Count < 3)
                {
                    int nextExamTypeId = _examService.GetNextExamType(applicant.CompletedExamTypes);
                    return RedirectToAction("StartExam", new { applicantId = applicant.ApplicantId.ToString(), examTypeId = nextExamTypeId });
                }

                return View();
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet]
        [Route("exam/result/{applicantId}")]
        public IActionResult Result(string applicantId)
        {
            // Parse the applicant ID
            var applicantGuid = Guid.Parse(applicantId);

            // Get the ApplicantExams for the specified applicant
            var applicantExams = _context.ApplicantExams
                .Where(ae => ae.ApplicantId == applicantGuid)
                .ToList();

            // Calculate the total score
            int totalScore = applicantExams.Sum(ae => ae.ApplicantScore ?? 0);

            // Pass the data to the view
            ViewBag.ApplicantExams = applicantExams;
            ViewBag.TotalScore = totalScore;

            return View("Result");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
