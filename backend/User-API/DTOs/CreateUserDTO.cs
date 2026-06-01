using System.ComponentModel.DataAnnotations;

namespace User_API.DTOs
{
    public class CreateUserDTO
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = null!;
        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = null!;
        [Required]
        [StringLength(50)]
        public string UserName { get; set; } = null!;
        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
    }
}
