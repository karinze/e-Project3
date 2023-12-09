using System.ComponentModel.DataAnnotations;

namespace AptitudeWebApp.Models
{
    public class ApplicantCompanies
    {
        [Key]
        public int ApplicantCompanyId { get; set; }
        public Guid ApplicantId { get; set; }
        [StringLength(255)]
        public string? CompanyName { get; set; }
        [StringLength(255)]
        public string? Description { get; set; }
        [StringLength(3000)]
        public string? Notes { get; set; }

        public DateTime? StartDate { get; set; }    
        public DateTime? EndDate { get; set; }
    }
}
