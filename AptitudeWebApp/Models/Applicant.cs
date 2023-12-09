using AptitudeWebApp.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AptitudeWebApp
{
    [Table("Applicants")]
    public class Applicant
    {
        [Key]
        public Guid ApplicantId { get; set; }
        [Display(Name = "First Name")]
        [Required(ErrorMessage = "First Name required")]
        [StringLength(255)]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        [StringLength(255)]

        public string? LastName { get; set; }
        public int? Age { get; set; }
        public bool? HasPassedExam { get; set; } = false;

        [Display(Name = "Last Name")]
        [StringLength(255)]
        public string? Address { get; set; }
        [StringLength(255)]
        public string? Email { get; set; }
        [StringLength(255)]
        public string? City { get; set; }

        [StringLength(3000)]
        public string? Notes { get; set; }
        public string? ImagePath { get; set; }

        [StringLength(512)]
        public string? Username { get; set; }
        [StringLength(512)]
        public string? Password { get; set; }
        public ICollection<ApplicantCompanies>? ApplicantCompanies { get; set; } = new List<ApplicantCompanies>();
        public ICollection<ApplicantEducation>? ApplicantEducation { get; set; } = new List<ApplicantEducation>();


    }
}
