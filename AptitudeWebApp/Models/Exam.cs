using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AptitudeWebApp.Models
{
    public class Exam
    {
        public Exam()
        {
            ExamQuestions = new List<ExamQuestions>();
            SelectedAnswers = new List<int>();
        }
        [Key]
        public int ExamId { get; set; }
        public Guid ApplicantId { get; set; }
        public int ExamTypeId { get; set; }
        public List<ExamQuestions>? ExamQuestions { get; set; }
        public List<int> SelectedAnswers { get; set; }
        public DateTime? StartTime { get; set; }
        public int TotalTimeAllowedInSeconds { get; set; } = 300;
        public int CurrentQuestionIndex { get; set; }

    }
}
