using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AptitudeWebApp.Models
{

    public class ExamQuestion
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        public int QuestionId { get; set; }

        [Required(ErrorMessage = "Type of Question must be set!")]
        public int ExamTypeId { get; set; } = 1;
        public int? QuestionScore { get; set; } = 0;
        [StringLength(3000)]
        public string? QuestionText { get; set; }
        [StringLength(512)]
        public string? QuestionA { get; set; }
        [StringLength(512)]

        public string? QuestionB { get; set; }
        [StringLength(512)]

        public string? QuestionC { get; set; }
        [StringLength(512)]

        public string? QuestionD { get; set; }
        [StringLength(512)]

        public string? CorrectQuestion { get; set; }
    }
}
