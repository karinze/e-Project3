using AptitudeWebApp.DAL;
using AptitudeWebApp.Models;
using AptitudeWebApp.Models.Authentication;
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
        [Authentication]
        public IActionResult StartExam1(Guid applicantId)
        {
            var questions = GetRandomQuestionsByExamType(1, 5);
            var exam = InitializeExam(applicantId, questions);
            return View("StartExam", exam);
        }

        [Authentication]
        public IActionResult StartExam2(Guid applicantId)
        {
            var questions = GetRandomQuestionsByExamType(2, 5);
            var exam = InitializeExam(applicantId, questions);
            return View("StartExam", exam);
        }

        [Authentication]
        public IActionResult StartExam3(Guid applicantId)
        {
            var questions = GetRandomQuestionsByExamType(3, 5);
            var exam = InitializeExam(applicantId, questions);
            return View("StartExam", exam);
        }
        [HttpPost]
        public IActionResult SubmitAnswer(Exam exam, int examTypeId)
        {

            // Process the selected answers, calculate the score, and update ApplicantExam
            int score = ProcessAnswers(exam);
            SaveExamScore(exam, score);
            var applicantExam = GetApplicantExamByApplicantIdAndExamId(exam.ApplicantId, exam.ExamId);

            applicantExam.ApplicantScore += score;

            // Mark the current exam type as completed
            applicantExam.CompletedExamTypes.Add(examTypeId);

            _context.ApplicantExams.Update(applicantExam);
            _context.SaveChanges();

            // Check if all three exam types are completed
            if (applicantExam.CompletedExamTypes.Count == 3)
            {
                // Display the result page with the total score
                return RedirectToAction("Result", new { applicantId = applicantExam.ApplicantId, totalScore = applicantExam.ApplicantScore });
            }

            // If not all three exams are done, redirect to the next exam type
            return RedirectToAction("Exam", new { applicantId = applicantExam.ApplicantId, examTypeId = GetNextExamType(applicantExam.CompletedExamTypes) });
        }
        private int GetNextExamType(List<int> completedExamTypes)
        {
            // Assuming exam types are 1, 2, and 3
            var allExamTypes = new List<int> { 1, 2, 3 };

            // Find the first exam type not completed
            var nextExamType = allExamTypes.Except(completedExamTypes).FirstOrDefault();

            return nextExamType;
        }
        public ApplicantExam GetApplicantExamByApplicantIdAndExamId(Guid applicantId, int examId)
        {
            return _context.ApplicantExams
                .FirstOrDefault(e => e.ApplicantId == applicantId && e.ExamId == examId);
        }
        //[HttpPost]
        //public IActionResult SubmitAnswer(Exam exam)
        //{
        //    //// Check if the time has expired
        //    //if (IsTimeExpired(exam.StartTime.Value, exam.TotalTimeAllowedInSeconds))
        //    //{
        //    //    // Handle time expiration
        //    //    return RedirectToAction("TimeExpired");
        //    //}

        //    // Process the submitted answers and calculate the score
        //    int score = ProcessAnswers(exam);

        //    // Save the score to the database
        //    SaveExamScore(exam, score);

        //    return RedirectToAction();
        //}
        private Answer GetCorrectAnswerForQuestion(int questionId)
        {
            // Retrieve the ExamQuestions entity for the given questionId
            var question = _context.ExamQuestions
                .Include(q => q.Answers) // Assuming you have navigation property Answers in ExamQuestions
                .FirstOrDefault(q => q.QuestionId == questionId);

            if (question != null)
            {
                // Return the correct answer for the question
                return question.Answers.FirstOrDefault(a => a.IsCorrect);
            }

            // Return null or handle the case when the question is not found
            return null;
        }
        private bool IsAnswerCorrect(ExamQuestions question, Answer selectedAnswer)
        {
            // Ensure that both the question and selectedAnswer are not null
            if (question == null || selectedAnswer == null)
            {
                return false;
            }

            return selectedAnswer.IsCorrect;
        }
        private Answer GetSelectedAnswerFromQuestion(ExamQuestions question, int selectedAnswerId)
        {
            // Ensure that the question is not null
            if (question == null)
            {
                return null;
            }

            // Find the selected answer in the question's answers
            return question.Answers.FirstOrDefault(answer => answer.Id == selectedAnswerId);
        }
        private int ProcessAnswers(Exam exam)
        {
            int score = 0;

            // Loop through each question in the exam
            for (var i = 0; i < exam.ExamQuestions.Count; i++)
            {
                // Get the correct answer for the current question from the database
                Answer correctAnswer = GetCorrectAnswerForQuestion(exam.ExamQuestions[i].QuestionId);
                Answer selectedAnswer = GetSelectedAnswerFromQuestion(exam.ExamQuestions[i], exam.SelectedAnswers[i]);
                // Check if the selected answer for the current question is correct
                if (IsAnswerCorrect(exam.ExamQuestions[i], selectedAnswer))
                {
                    score++; // Increment the total score
                }
                else
                {
                    // The answer is incorrect
                }
            }

            return score;
        }

        private void SaveExamScore(Exam exam, int score)
        {
            // Replace this with your actual logic to save the exam score to the database
            // You might want to use the ApplicantExam model to store the score
            var applicantExam = new ApplicantExam
            {
                ApplicantId = exam.ApplicantId,
                ExamId = exam.ExamId,
                ApplicantScore = score,
                CurrentExamTypeId = exam.ExamTypeId
            };

            // Save to the database using your DbContext
            _context.ApplicantExams.Add(applicantExam);
            _context.SaveChanges();
        }
        public IActionResult Result(Guid applicantId)
        {
            // Get the ApplicantExams for the specified applicant
            var applicantExams = _context.ApplicantExams
                .Where(ae => ae.ApplicantId == applicantId)
                .ToList();

            // Calculate the total score
            int totalScore = applicantExams.Sum(ae => ae.ApplicantScore ?? 0);

            // Pass the data to the view
            ViewBag.ApplicantExams = applicantExams;
            ViewBag.TotalScore = totalScore;

            return View();
        }
        private List<ExamQuestions> GetRandomQuestionsByExamType(int examTypeId, int numberOfQuestions)
        {
            var questions = _context.ExamQuestions
                .Where(q => q.ExamTypeId == examTypeId)
                .OrderBy(x => Guid.NewGuid()) // Randomize the order
                .Take(numberOfQuestions)
                .ToList();

            return questions;
        }
        private Exam InitializeExam(Guid applicantId, List<ExamQuestions> questions)
        {
            return new Exam
            {
                ApplicantId = applicantId,
                StartTime = DateTime.Now,
                CurrentQuestionIndex = 0,
                ExamQuestions = questions,
                SelectedAnswers = new List<int>(),
                TotalTimeAllowedInSeconds = 300 // Set the default total time allowed for the entire exam
            };
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
        //[HttpPost]
        //[Route("submit")]
        //public IActionResult Submit(List<int> answers)
        //{
        //    var questions = _context.ExamQuestions.Include(q => q.Answers).ToList();
        //    var totalQuestions = questions.Count;
        //    var correctAnswers = 0;

        //    for (int i = 0; i < totalQuestions; i++)
        //    {
        //        var selectedAnswerId = answers[i];
        //        var correctAnswerId = questions[i].Answers.FirstOrDefault(a => a.IsCorrect)?.Id;

        //        if (selectedAnswerId == correctAnswerId)
        //        {
        //            correctAnswers++;
        //        }
        //    }

        //    var score = (correctAnswers * 100) / totalQuestions;

        //    ViewBag.Score = score;

        //    return View("Result");

        //}

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
