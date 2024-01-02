using System.ComponentModel.DataAnnotations;

namespace AptitudeWebApp.Models
{
    public class ApplicantExam
    {
        [Key]
        public int ApplicantExamId { get; set; }
        public Guid ApplicantId { get; set; }
        public int ExamId { get; set; }
        public int? ApplicantScore { get; set; } = 0;
        public int CurrentExamTypeId { get; set; } = 1;

    }
}
