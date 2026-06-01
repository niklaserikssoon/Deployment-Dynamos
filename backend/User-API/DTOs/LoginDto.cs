using System.ComponentModel.DataAnnotations;

namespace User_API.DTOs
{
    public class LoginDTO
    {
        [Required]
        [StringLength(50)]
        public string UserName { get; set; } = null!;

        [Required]
        [StringLength(255)]
        public string Password { get; set; } = null!;
    }
}