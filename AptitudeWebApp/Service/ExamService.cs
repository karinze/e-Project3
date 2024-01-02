using AptitudeWebApp.DAL;
using AptitudeWebApp.Models;
using AptitudeWebApp.Repository;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using System.Linq;

namespace AptitudeWebApp.Service
{
    public class ExamService : IExamService
    {
        private readonly AptitudeContext _context;

        public ExamService(AptitudeContext context)
        {
            _context = context;
        }
        public List<ExamQuestions> GetRandomQuestionsByExamType(int examTypeId, int numberOfQuestions)
        {
            var questions = _context.ExamQuestions
                .Where(q => q.ExamTypeId == examTypeId)
                .OrderBy(x => Guid.NewGuid()) // Randomize the order
                .Take(numberOfQuestions)
                .Distinct()
                .ToList();
            HashSet<ExamQuestions> tempQuestions = new HashSet<ExamQuestions>();
            tempQuestions.AddRange(questions);
            foreach (var single in tempQuestions)
            {
                // Magic initialization (call a list it automatically adds to the object????)
                var answers = _context.Answers.Where(x => x.QuestionId == single.QuestionId).ToList();
            }
            return tempQuestions.ToList();
        }
        public Exam InitializeExam(Guid applicantId, List<ExamQuestions> questions, int examTypeId)
        {
            var newExam = new Exam
            {
                ApplicantId = applicantId,
                StartTime = DateTime.Now,
                CurrentQuestionIndex = 0,
                ExamQuestions = questions,
                SelectedAnswers = new List<int>(),
                ExamTypeId = examTypeId,
                TotalTimeAllowedInSeconds = 60 // Set the default total time allowed for the entire exam
            };
            _context.Exams.Add(newExam);
            _context.SaveChanges();
            return newExam;
        }
        public int GetNextExamType(List<int> completedExamTypes)
        {
            // Assuming exam types are 1, 2, and 3
            var allExamTypes = new List<int> { 1, 2, 3 };

            // Find the first exam type not completed
            var nextExamType = allExamTypes.Except(completedExamTypes).FirstOrDefault();

            return nextExamType;
        }
        public Applicant GetApplicantByApplicantId(Guid applicantId)
        {
            return _context.Applicants.FirstOrDefault(x => x.ApplicantId == applicantId);
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
        public Answer GetCorrectAnswerForQuestion(int questionId)
        {
            // Retrieve the ExamQuestions entity for the given questionId
            var question = _context.ExamQuestions
                .Include(q => q.Answers)
                .FirstOrDefault(q => q.QuestionId == questionId);

            if (question != null)
            {
                // Return the correct answer for the question
                return question.Answers.FirstOrDefault(a => a.IsCorrect);
            }

            // Return null or handle the case when the question is not found
            return null;
        }
        public bool IsAnswerCorrect(ExamQuestions question, Answer selectedAnswer)
        {
            // Ensure that both the question and selectedAnswer are not null
            if (question == null || selectedAnswer == null)
            {
                return false;
            }

            return selectedAnswer.IsCorrect;
        }
        public Answer GetSelectedAnswerFromQuestion(ExamQuestions question, int selectedAnswerId)
        {
            // Ensure that the question is not null
            if (question == null)
            {
                return null;
            }

            // Find the selected answer in the question's answers
            return question.Answers.FirstOrDefault(answer => answer.Id == selectedAnswerId);
        }
        public int ProcessAnswers(Exam exam)
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
                    // The answer is incorrect, return nothing
                }
            }

            return score;
        }

        public void SaveExamScore(Exam exam, int score)
        {
            var currentApplicant = GetApplicantByApplicantId(exam.ApplicantId);

            var applicantExam = new ApplicantExam
            {
                ApplicantId = exam.ApplicantId,
                ExamId = exam.ExamId,
                ApplicantScore = score,
                CurrentExamTypeId = exam.ExamTypeId
            };
            currentApplicant.CompletedExamTypes.Add(exam.ExamTypeId);
            _context.Applicants.Update(currentApplicant);
            _context.ApplicantExams.Add(applicantExam);
            _context.SaveChanges();
        }
    }
}
