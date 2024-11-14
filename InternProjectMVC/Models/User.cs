using System.ComponentModel.DataAnnotations;

namespace InternProjectMVC.Models
{
    public class User
    {
        public enum Role 
        { 
            User,
            Admin
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50,ErrorMessage = "Cannot enter more than 50 characters.")]
        public required string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Cannot enter more than 100 characters.")]
        public required string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Cannot enter more than 100 characters.")]
        public required string Password { get; set; }

        [Required]
        public Role UserRole { get; set; }
    }
}
