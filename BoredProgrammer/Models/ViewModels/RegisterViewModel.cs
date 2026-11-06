using System.ComponentModel.DataAnnotations;

namespace BoredProgrammer.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage ="Email is required")]
        [EmailAddress(ErrorMessage ="Email must be in proper format")]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Email is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        [Compare("Password", ErrorMessage = "Password & Confirm Password must be Same")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
