using System.ComponentModel.DataAnnotations;

namespace InternProjectMVC.Models
{
    public class Permission
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public required string UserName { get; set; }

        [Required]
        [StringLength(50)]
        public required string ProgramName { get; set; }

        [Required]
        [StringLength(20)]
        public required string role { get; set; }

        [Required]
        [StringLength(50)]
        public required string Action {  get; set; }
    }
}
