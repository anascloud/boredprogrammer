using System.ComponentModel.DataAnnotations;

namespace BoredProgrammer.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is a Required Field")]
        [EmailAddress(ErrorMessage = "Email must be in proper format")]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Password is a Required Field")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}