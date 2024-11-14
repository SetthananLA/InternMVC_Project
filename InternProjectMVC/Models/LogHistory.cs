using System.ComponentModel.DataAnnotations;

namespace InternProjectMVC.Models
{
    public class LogHistory
    {
        public enum Status
        {
            success,
            fail
        }
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "Cannot enter more than 50 characters.")]
        public required string Email { get; set; }

        [Required]
        [StringLength(20)]
        public required string IpAddress { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public required DateTime LoginDate { get; set; }

        [Required]
        public required Status LoginStatus { get; set; }
    }
}
