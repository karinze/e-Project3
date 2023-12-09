using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AptitudeWebApp
{
    public class Manager
    {
        [Key]
        public Guid ManagerId { get; set; }
        [Display(Name = "")]
        [Required(ErrorMessage = "Name required")]
        [StringLength(255)]

        public string? FirstName { get; set; }
        [StringLength(255)]

        public string? LastName { get; set; }

        [Required(ErrorMessage = "Username required")]
        [StringLength(512)]

        public string Username { get; set; }
        [Required(ErrorMessage = "Password required")]
        [StringLength(512)]

        public string Password { get; set; }



    }
}
