using AptitudeWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AptitudeWebApp
{
    [Table("Applicants")]
    
    public class Applicant
    {

        public Applicant() {
            ApplicantCompanies = new List<ApplicantCompanies>();
            ApplicantEducation = new List<ApplicantEducation>();
            CompletedExamTypes = new List<int>();
        }  
        [Key]
        public Guid ApplicantId { get; set; }
        [Display(Name = "First Name")]
        [Required(ErrorMessage = "First Name required")]
        [StringLength(255)]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        [StringLength(255)]
        [Required(ErrorMessage = "Last Name required")]
        public string? LastName { get; set; }
        [Required(ErrorMessage = "Age required")]
        [Range(18, 60, ErrorMessage = "Can only be between 18 ands 60")]
        public int? Age { get; set; }

        public bool? HasPassedExam { get; set; } = false;
        [Required(ErrorMessage = "Address required")]
        [StringLength(255)]
        public string? Address { get; set; }
        [Required(ErrorMessage = "Email required")]
        [StringLength(255)]
        [EmailAddress(ErrorMessage = "Enter valid email")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "City required")]
        [StringLength(255)]
        public string? City { get; set; }
        [Required(ErrorMessage = "Phone Number required")]
        [StringLength(255)]
        [RegularExpression("^([0-9]{10})$",ErrorMessage ="Invalid Phone Number.")]
        public string? PhoneNumber { get; set; }
        [Required(ErrorMessage = "Notes required")]
        [StringLength(3000)]
        public string? Notes { get; set; }
        public string? ImagePath { get; set; }
        [Required(ErrorMessage = "Username must exist")]
        [StringLength(512)]
        [Remote("IsUserName","Manager",ErrorMessage ="User Name already available")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Password must exist")]
        [StringLength(512)]
        
        public string Password { get; set; }
        public ICollection<ApplicantCompanies>? ApplicantCompanies { get; set; }
        public ICollection<ApplicantEducation>? ApplicantEducation { get; set; }
        public List<int>? CompletedExamTypes { get; set; }


    }
}
