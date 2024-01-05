using AptitudeWebApp.Models;

namespace AptitudeWebApp.Repository
{
    public interface IExamService
    {
        List<Questions> GetRandomQuestionsByExamType(int examTypeId, int numberOfQuestions);
        Exam InitializeExam(Guid applicantId, List<Questions> questions, int examTypeId);
        int GetNextExamType(List<int> completedExamTypes);
        ApplicantExam GetApplicantExamByApplicantIdAndExamId(Guid applicantId, int examId);
        Answer GetCorrectAnswerForQuestion(int questionId);
        bool IsAnswerCorrect(Questions question, Answer selectedAnswer);
        Answer GetSelectedAnswerFromQuestion(Questions question, int selectedAnswerId);
        int ProcessAnswers(Exam exam);
        void SaveExamScore(Exam exam, int score);
        Applicant GetApplicantByApplicantId(Guid applicantId);
        Exam GetExamDetails(int examId);
    }
}