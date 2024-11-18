using System.ComponentModel.DataAnnotations;

namespace InternProjectMVC.Models
{
    public class Certified
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public required string UserName { get; set; }

        [Required]
        [StringLength(50)]
        public required string AuthenBy { get; set; }

        [Required]
        [StringLength(50)]
        public required string Status { get; set; }
    }
}
