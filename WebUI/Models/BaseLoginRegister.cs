using System.ComponentModel.DataAnnotations;

namespace WebUI.Models
{
    public class BaseLoginRegister
    {
        [Required(ErrorMessage = "Username is required!")]
        [StringLength(30, ErrorMessage = "Username should be max 30 characters!")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required!")]
        [MinLength(6, ErrorMessage = "Password should be min 6 characters!")]
        [MaxLength(16, ErrorMessage = "Password should be max 16 characters!")]
        public string Password { get; set; }
    }
}