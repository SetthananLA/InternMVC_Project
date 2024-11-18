using System.ComponentModel.DataAnnotations;

namespace InternProjectMVC.Models
{
    public class Action
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string Pid { get; set; }

        [Required]
        [StringLength(50)]
        public required string ActionName { get; set; }
    }
}
