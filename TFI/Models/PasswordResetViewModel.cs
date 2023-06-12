using System.ComponentModel.DataAnnotations;

namespace TFI.Models
{
    public class PasswordResetViewModel
    {
        [Required]
        public string Token { get; set; }

        [Required]
        public string Password { get; set; }
        
        [Required]
        public string RepeatPassword { get; set; }
    }
}