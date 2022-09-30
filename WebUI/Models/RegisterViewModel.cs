using System.ComponentModel.DataAnnotations;

namespace WebUI.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Username is required!")]
        [StringLength(30, ErrorMessage = "Username should be max 30 characters!")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required!")]
        [MinLength(6, ErrorMessage = "Password should be min 6 characters!")]
        [MaxLength(16, ErrorMessage = "Password should be max 16 characters!")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Re-Password is required!")]
        [MinLength(6, ErrorMessage = "Re-Password should be min 6 characters!")]
        [MaxLength(16, ErrorMessage = "Re-Password should be max 16 characters!")]
        [Compare(nameof(Password), ErrorMessage = "The passwords do not match")]
        public string RePassword { get; set; }
    }
}
