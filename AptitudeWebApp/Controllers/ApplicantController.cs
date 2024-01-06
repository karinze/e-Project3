using AptitudeWebApp.DAL;
using AptitudeWebApp.Models;
using AptitudeWebApp.Models.Authentication;
using AptitudeWebApp.Repository;
using AptitudeWebApp.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text.Json;

namespace AptitudeWebApp.Controllers
{
    public class ApplicantController : Controller
    {
        private readonly AptitudeContext _context;
        private readonly IExamService _examService;
        private readonly ILogger<ApplicantController> _logger;
        private readonly IHttpContextAccessor _httpContext;


        public ApplicantController(AptitudeContext context, IExamService examService, ILogger<ApplicantController> logger, IHttpContextAccessor httpContext)
        {
            _context = context;
            _examService = examService;
            _logger = logger;
            _httpContext = httpContext;
        }
        public IActionResult Index()
        {
            try
            {
                return View();

            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred in StartExam. Error message: {ex.Message}");
                return View("NotFound");
            }
        }

        public IActionResult ExamAlreadyCompleted()
        {
            try
            {
                return View();

            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred in StartExam. Error message: {ex.Message}");
                return View("NotFound");
            }
        }
        public IActionResult ExamDashboard()
        {
            try
            {
                // Check if Applicant has logged in
                if (HttpContext.Session.GetString("Applicant") == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred in StartExam. Error message: {ex.Message}");
                return View("NotFound");
            }

        }
        //[HttpPost]
        //[ValidateExam]
        //[Route("Exam/start/{applicantId}/{examTypeId:int}")]
        //public IActionResult StartExam(string applicantId, int examTypeId)
        //{
        //    var applicantGuid = Guid.Parse(applicantId);
        //    if (HttpContext.Session.GetString("Applicant") == null)
        //    {
        //        return RedirectToAction("Login", "Account");
        //    }
        //    // Get 5 random questions with the exam type along with their answers
        //    var questions = _examService.GetRandomQuestionsByExamType(examTypeId, 5);

        //    // Initialize the Exam parameters
        //    var exam = _examService.InitializeExam(applicantGuid, questions, examTypeId);

        //    //exam.ExamQuestions.AddRange(questions);
        //    return View("Exam", exam);
        //}


        [HttpPost]
        [HttpGet]
        [ValidateExam]
        [Route("exam/start/{applicantId}/{examTypeId:int}")]
        public IActionResult StartExam(string applicantId, int examTypeId)
        {
            try
            {
                var applicantGuid = Guid.Parse(applicantId);

                if (HttpContext.Session.GetString("Applicant") == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var currentExam = HttpContext.Session.Get<Exam>($"CurrentExam_{examTypeId}");

                if (currentExam == null || currentExam.ExamTypeId != examTypeId)
                {
                    // If there is no current exam or the exam type is different, create a new exam
                    var questions = _examService.GetRandomQuestionsByExamType(examTypeId, 5);
                    currentExam = _examService.InitializeExam(applicantGuid, questions, examTypeId);

                    HttpContext.Session.Set($"CurrentExam_{examTypeId}", currentExam);
                }

                currentExam.StartTime = DateTime.Now;

                HttpContext.Session.Set($"CurrentExam_{examTypeId}", currentExam);

                return View("Exam", currentExam);

            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred in StartExam. Error message: {ex.Message}");
                return View("NotFound");
            }

        }

        [HttpPost]
        public IActionResult UpdateTimer(int examId)
        {
            try
            {
                var currentExam = _context.Exams.FirstOrDefault(x => x.ExamId == examId);

                if (currentExam == null)
                {
                    return Json(new { isExamEnded = true });
                }

                var elapsedTimeInSeconds = (int)(DateTime.Now - currentExam.StartTime).Value.TotalSeconds;
                var remainingTime = Math.Max(currentExam.TotalTimeAllowedInSeconds - elapsedTimeInSeconds, 0);
                // Check if time has expired
                if (remainingTime <= 0)
                {
                    // Set the exam as ended
                    return Json(new { isExamEnded = true });
                }

                // Return remaining time to the client
                return Json(new { isExamEnded = false, remainingTime = remainingTime });
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred in StartExam. Error message: {ex.Message}");
                return View("NotFound");
            }

        }
        [HttpPost]
        public IActionResult SubmitAnswer(string applicantId, int examId, int examTypeId, List<int> SelectedAnswers)
        {
            try
            {
                Exam exam = _context.Exams.FirstOrDefault(x => x.ExamId == examId);
                var applicantGuid = Guid.Parse(applicantId);

                if (exam != null)
                {
                    exam.SelectedAnswers = SelectedAnswers;
                    int score = _examService.ProcessAnswers(exam);

                    _examService.SaveExamScore(exam, score);

                    var applicant = _examService.GetApplicantByApplicantId(applicantGuid);

                    // Check if all three exam types are completed
                    if (applicant.CompletedExamTypes.Contains(1) && applicant.CompletedExamTypes.Contains(2) && applicant.CompletedExamTypes.Contains(3))
                    {
                        var applicantExams = _context.ApplicantExams.Where(x => x.ApplicantId == applicant.ApplicantId).ToList();
                        if (applicantExams.Sum(x => x.ApplicantScore) > 12)
                        {
                            applicant.HasPassedExam = true;
                        }
                        else
                        {
                            applicant.HasPassedExam = false;
                        }
                        _context.Applicants.Update(applicant);
                        _context.SaveChangesAsync();
                        _httpContext.HttpContext.Session.Remove("CurrentExam");

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
                    return View("NotFound");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred in StartExam. Error message: {ex.Message}");
                return View("NotFound");
            }

        }

        [HttpGet]
        [Route("exam/result/{applicantId}")]
        public IActionResult Result(string applicantId)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred in StartExam. Error message: {ex.Message}");
                return View("NotFound");
            }

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            try
            {
                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred in StartExam. Error message: {ex.Message}");
                return View("NotFound");
            }
        }
    }
}
