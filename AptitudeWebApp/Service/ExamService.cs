using AptitudeWebApp.DAL;
using AptitudeWebApp.Models;
using AptitudeWebApp.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NuGet.Packaging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AptitudeWebApp.Service
{
    public class ExamService : IExamService
    {
        private readonly AptitudeContext _context;
        private readonly ILogger<ExamService> _logger;
        private readonly IHttpContextAccessor _httpContext;

        public ExamService(AptitudeContext context, ILogger<ExamService> logger, IHttpContextAccessor httpContext)
        {
            _context = context;
            _logger = logger;
            _httpContext = httpContext;
        }

        public List<Questions> GetRandomQuestionsByExamType(int examTypeId, int numberOfQuestions)
        {
            try
            {
                var questions = _context.Questions
                    .Where(q => q.ExamTypeId == examTypeId)
                    .OrderBy(x => Guid.NewGuid())
                    .Take(numberOfQuestions)
                    .Distinct()
                    .ToList();

                // Make hashset to make sure there are no duplicates (hopefully)
                HashSet<Questions> tempQuestions = new HashSet<Questions>();
                tempQuestions.AddRange(questions);
                foreach (var single in tempQuestions)
                {
                    // Add answers
                    var answers = _context.Answers.Where(x => x.QuestionId == single.QuestionId).ToList();
                    single.Answers = answers;
                }
                return tempQuestions.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetRandomQuestionsByExamType: {ex.Message}");
                throw;
            }
        }

        public Exam InitializeExam(Guid applicantId, List<Questions> questions, int examTypeId)
        {
            try
            {
                var newExam = new Exam
                {
                    ApplicantId = applicantId,
                    StartTime = DateTime.Now,
                    CurrentQuestionIndex = 0,
                    ExamQuestions = questions,
                    SelectedAnswers = new List<int>(),
                    ExamTypeId = examTypeId,
                    TotalTimeAllowedInSeconds = 180 // Set the default total time allowed for the entire exam
                };
                _context.Exams.Add(newExam);
                _context.SaveChanges();

                // Create new Exam to get its Id for the ExamQuestions below.
                foreach (var question in questions)
                {
                    var examQuestion = new ExamQuestions
                    {
                        ExamId = newExam.ExamId,
                        QuestionId = question.QuestionId,
                    };
                    _context.ExamQuestions.Add(examQuestion);
                    _context.SaveChanges();
                }

                return newExam;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in InitializeExam: {ex.Message}");
                throw;
            }
        }

        public int GetNextExamType(List<int> completedExamTypes)
        {
            try
            {
                // Assuming exam types are 1, 2, and 3
                var allExamTypes = new List<int> { 1, 2, 3 };

                // Find the first exam type not completed
                var nextExamType = allExamTypes.Except(completedExamTypes).FirstOrDefault();

                return nextExamType;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetNextExamType: {ex.Message}");
                throw;
            }
        }
        public Exam GetExamDetails(int examId)
        {
            try
            {
                return _context.Exams.FirstOrDefault(x => x.ExamId == examId);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetApplicantByApplicantId: {ex.Message}");
                throw;
            }
        }

        public Applicant GetApplicantByApplicantId(Guid applicantId)
        {
            try
            {
                return _context.Applicants.FirstOrDefault(x => x.ApplicantId == applicantId);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetApplicantByApplicantId: {ex.Message}");
                throw;
            }
        }

        public ApplicantExam GetApplicantExamByApplicantIdAndExamId(Guid applicantId, int examId)
        {
            try
            {
                return _context.ApplicantExams
                    .FirstOrDefault(e => e.ApplicantId == applicantId && e.ExamId == examId);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetApplicantExamByApplicantIdAndExamId: {ex.Message}");
                throw;
            }
        }

        public Answer GetCorrectAnswerForQuestion(int questionId)
        {
            try
            {
                // Retrieve the ExamQuestions entity for the given questionId
                var question = _context.Questions
                    .Include(q => q.Answers)
                    .FirstOrDefault(q => q.QuestionId == questionId);

                if (question != null)
                {
                    // Return the correct answer for the question
                    return question.Answers.FirstOrDefault(a => a.IsCorrect);
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetCorrectAnswerForQuestion: {ex.Message}");
                throw;
            }
        }

        public bool IsAnswerCorrect(Questions question, Answer selectedAnswer)
        {
            try
            {
                if (question == null || selectedAnswer == null)
                {
                    return false;
                }

                return selectedAnswer.IsCorrect;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in IsAnswerCorrect: {ex.Message}");
                throw;
            }
        }

        public Answer GetSelectedAnswerFromQuestion(Questions question, int selectedAnswerId)
        {
            try
            {
                if (question == null)
                {
                    return null;
                }

                // Find the selected answer in the question's answers
                return question.Answers.FirstOrDefault(answer => answer.Id == selectedAnswerId);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetSelectedAnswerFromQuestion: {ex.Message}");
                throw;
            }
        }

        public int ProcessAnswers(Exam exam)
        {
            try
            {
                int score = 0;
                int i = 0;

                var examQuestions = _context.ExamQuestions
                .Where(x => x.ExamId == exam.ExamId)
                .ToList(); 

                var questionIdsInOrder = examQuestions.Select(eq => eq.QuestionId).ToList();

                var questions = _context.Questions
                    .Where(q => questionIdsInOrder.Contains(q.QuestionId))
                    .ToList() 
                    .OrderBy(q => questionIdsInOrder.IndexOf(q.QuestionId));

                // Loop through each question in the exam
                foreach (var question in questions)
                {
                    var answers = _context.Answers.Where(x => x.QuestionId == question.QuestionId);
                    question.Answers = answers.ToList();
                    if (i < exam.SelectedAnswers.Count)
                    {
                        Answer selectedAnswer = GetSelectedAnswerFromQuestion(question, exam.SelectedAnswers[i]);

                        if (i < exam.SelectedAnswers.Count)
                        {
                            if (selectedAnswer.IsCorrect)
                            {
                                score++;
                            }
                        }
                    }   
                    else
                    {

                    }
                    i++;

                }

                return score;

                //// Get the correct answer for the current question from the database
                //Answer correctAnswer = GetCorrectAnswerForQuestion(question.QuestionId);

                //var examQuestion = examQuestions.FirstOrDefault(eq => eq.QuestionId == question.QuestionId);

                //if (examQuestion != null && i < exam.SelectedAnswers.Count)
                //{
                //    Answer selectedAnswer = GetSelectedAnswerFromQuestion(question, exam.SelectedAnswers[i]);
                //    // Check if the selected answer for the current question is correct
                //    if (IsAnswerCorrect(question, selectedAnswer))
                //    {
                //        score++; // Increment the total score
                //    }
                //    else
                //    {
                //        // The answer is incorrect, return nothing
                //    }
                //}
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in ProcessAnswers: {ex.Message}");
                throw;
            }
        }

        public void SaveExamScore(Exam exam, int score)
        {
            try
            {
                var currentApplicant = GetApplicantByApplicantId(exam.ApplicantId);
                var currentSession = _httpContext.HttpContext.Session;
                // Create new ApplicantExam once we Submit each Exam.
                var applicantExam = new ApplicantExam
                {
                    ApplicantId = exam.ApplicantId,
                    ExamId = exam.ExamId,
                    ApplicantScore = score,
                    CurrentExamTypeId = exam.ExamTypeId
                };

                // If Applicant doesn't contain the finished Exam Type, insert it for checking later
                if (!currentApplicant.CompletedExamTypes.Contains(exam.ExamTypeId))
                {
                    currentApplicant.CompletedExamTypes.Add(exam.ExamTypeId);
                    currentSession.Remove("CompletedExamTypes");
                    currentSession.Set<List<int>>("CompletedExamTypes", currentApplicant.CompletedExamTypes);
                }
                _context.Applicants.Update(currentApplicant);
                _context.ApplicantExams.Add(applicantExam);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in SaveExamScore: {ex.Message}");
                throw;
            }
        }
    }
}
