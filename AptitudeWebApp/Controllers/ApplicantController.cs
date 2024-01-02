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
        //private readonly IGenericRepository<Exam> _db;
        private readonly AptitudeContext _context;
        private readonly IExamService _examService;

        ApplicantExam apExam = new ApplicantExam(); //initialize new ApplicantExam
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
            var questions = _examService.GetRandomQuestionsByExamType(examTypeId, 5);
            var exam = _examService.InitializeExam(applicantGuid, questions, examTypeId);
            exam.ExamQuestions.AddRange(questions);
            return View("Exam", exam);
        }
        
        [HttpPost]
        [Route("exam/submit/{applicantId}/{examTypeId:int}")]
        public IActionResult SubmitAnswer(Exam exam, int examTypeId)
        {

            // Process the selected answers, calculate the score, and update ApplicantExam
            int score = _examService.ProcessAnswers(exam);
            _examService.SaveExamScore(exam, score);
            var applicantExam = _examService.GetApplicantExamByApplicantIdAndExamId(exam.ApplicantId, exam.ExamId);

            applicantExam.ApplicantScore += score;

            // Mark the current exam type as completed
            applicantExam.CompletedExamTypes.Add(examTypeId);

            _context.ApplicantExams.Update(applicantExam);
            _context.SaveChanges();

            // Check if all three exam types are completed
            if (applicantExam.CompletedExamTypes.Count == 3)
            {
                // Display the result page with the total score
                return RedirectToAction("Result", new { applicantId = applicantExam.ApplicantId });
            }
            // If not all three exams are done, redirect to the next exam type
            else if (applicantExam.CompletedExamTypes.Count < 3)
            {
                int nextExamTypeId = _examService.GetNextExamType(applicantExam.CompletedExamTypes);
                return RedirectToAction("StartExam", new { applicantId = applicantExam.ApplicantId.ToString(), examTypeId = nextExamTypeId });
            }

            return View();
        }
        
        [HttpGet]
        [Route("exam/result/{applicantId}")]
        public IActionResult Result(string applicantId)
        {
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
