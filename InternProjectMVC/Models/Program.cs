using System.ComponentModel.DataAnnotations;

namespace InternProjectMVC.Models
{
    public class Program
    {
        [Key]
        public required string Pid { get; set; }

        [Required]
        [StringLength(50)]
        public required string ProgramName { get; set; }
    }
}
