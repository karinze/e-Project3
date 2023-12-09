using System.ComponentModel.DataAnnotations;

namespace AptitudeWebApp.Models
{
    public class ExamCategory
    {
        [Key]
        public int CategoryId { get; set; }
        [StringLength(255)]
        public string? CategoryName { get; set; }
        public ICollection<ExamQuestion>? ExamQuestions { get; set; } = new List<ExamQuestion>();
                      

    }
}
