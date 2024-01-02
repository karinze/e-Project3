using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AptitudeWebApp.Models
{
    public class Exam
    {
        public Exam()
        {
            ExamQuestions = new List<ExamQuestion>();
        }
        [Key]
        public int ExamId { get; set; }
        public int ExamTypeId { get; set; }
        public ICollection<ExamQuestion>? ExamQuestions { get; set; } 
        public DateTime? StartTime { get; set; }

        public bool IsActive { get; set; } = false;

    }
}
