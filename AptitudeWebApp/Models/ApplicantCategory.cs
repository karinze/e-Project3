using System.ComponentModel.DataAnnotations;

namespace AptitudeWebApp.Models
{
    public class ApplicantCategory
    {
        [Key]
        public int ApplicantCategoryId { get; set; }
        public Guid ApplicantId {  get; set; }
        public int? CategoryId { get; set; }
        public int Score { get; set; } = 0;

        public bool HasCleared { get; set; } = false;
    }
}
