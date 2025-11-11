using System.ComponentModel.DataAnnotations;

namespace FUNewsManagement.Models
{
    public class RegisterViewModel
    {
        [Required, StringLength(100)]
        public string Name { get; set; } = null!;

        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required, DataType(DataType.Password), MinLength(6)]
        public string Password { get; set; } = null!;

        [Required, DataType(DataType.Password), Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = null!;
    }
}
