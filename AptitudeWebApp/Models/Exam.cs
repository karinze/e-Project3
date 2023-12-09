using System.ComponentModel.DataAnnotations;

namespace AptitudeWebApp.Models
{
    public class Exam
    {
        [Key]
        public int ExamId { get; set; }
        public int ApplicantId { get; set; }
        public ICollection<ExamCategory>? ExamCategories { get; set; } = new List<ExamCategory>();
        public DateTime? StartTime { get; set; }    
        public DateTime? EndTime {  get; set; }
        
        public int HasStartedYet { get; set; }

    }
}
