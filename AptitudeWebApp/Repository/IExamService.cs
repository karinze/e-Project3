using AptitudeWebApp.Models;

namespace AptitudeWebApp.Repository
{
    public interface IExamService
    {
        List<ExamQuestions> GetRandomQuestionsByExamType(int examTypeId, int numberOfQuestions);
        Exam InitializeExam(Guid applicantId, List<ExamQuestions> questions, int examTypeId);
        int GetNextExamType(List<int> completedExamTypes);
        ApplicantExam GetApplicantExamByApplicantIdAndExamId(Guid applicantId, int examId);
        Answer GetCorrectAnswerForQuestion(int questionId);
        bool IsAnswerCorrect(ExamQuestions question, Answer selectedAnswer);
        Answer GetSelectedAnswerFromQuestion(ExamQuestions question, int selectedAnswerId);
        int ProcessAnswers(Exam exam);
        void SaveExamScore(Exam exam, int score);
    }
}