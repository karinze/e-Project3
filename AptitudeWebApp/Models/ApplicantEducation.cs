using System.ComponentModel.DataAnnotations;

namespace AptitudeWebApp.Models
{
    public class ApplicantEducation
    {
        [Key]
        public int ApplicantEducationId { get; set; }
        public Guid ApplicantId { get; set; }
        [StringLength(255)]
        public string? COEName { get; set; }
        [StringLength(2000)]
        public string? Description { get; set; }
        [StringLength(3000)]
        public string? Notes { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
