using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AptitudeWebApp.Models
{

    public class Questions
    {
        public Questions()
        {
            Answers = new List<Answer>();
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]

        public int QuestionId { get; set; }
        [Required(ErrorMessage = "Type of Question must be set!")]
        public int ExamTypeId { get; set; } = 1;

        public int? QuestionScore { get; set; } = 0;
        public string? QuestionText { get; set; }
        public List<Answer> Answers { get; set; }
    }
}
