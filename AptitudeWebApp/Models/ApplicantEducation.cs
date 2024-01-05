using System.ComponentModel.DataAnnotations;

namespace AptitudeWebApp.Models
{
    public class ApplicantEducation
    {
        [Key]
        public int ApplicantEducationId { get; set; }
        public Guid ApplicantId { get; set; }
        [Required(ErrorMessage = "COE Name required")]
        [StringLength(255)]
        public string? COEName { get; set; }
        [Required(ErrorMessage = "Description required")]
        [StringLength(255)]
        public string? Description { get; set; }
        //[Required(ErrorMessage = "Notes required")]
        [StringLength(3000)]
        public string? Notes { get; set; }
        [Required(ErrorMessage = "Start Date required")]

        public DateTime? StartDate { get; set; }
        [Required(ErrorMessage = "End Date required")]
        public DateTime? EndDate { get; set; }
    }
}
