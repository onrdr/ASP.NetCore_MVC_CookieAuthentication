using System.ComponentModel.DataAnnotations;

namespace WebUI.Models
{
    public class RegisterViewModel : BaseLoginRegister
    {  
        [Required(ErrorMessage = "Re-Password is required!")]
        [MinLength(6, ErrorMessage = "Re-Password should be min 6 characters!")]
        [MaxLength(16, ErrorMessage = "Re-Password should be max 16 characters!")]
        [Compare(nameof(base.Password), ErrorMessage = "The passwords do not match")]
        public string RePassword { get; set; }
    }
}
