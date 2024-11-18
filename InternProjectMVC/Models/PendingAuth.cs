using System.ComponentModel.DataAnnotations;

namespace InternProjectMVC.Models
{
    public class PendingAuth
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public required string UserName { get; set; }

        [Required]
        [StringLength(20)]
        public required string UserRole { get; set; }
    }
}
