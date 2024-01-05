using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AptitudeWebApp.Models
{
    public class ExamQuestions
    {
        [Key]
        public int ExamQuestionsId { get; set; }
        public int ExamId { get; set; }

        public int QuestionId { get; set; }
    }
}
