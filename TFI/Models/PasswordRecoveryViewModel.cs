using System.ComponentModel.DataAnnotations;

namespace TFI.Models
{
    public class PasswordRecoveryViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string TipoUtente { get; set; }
    }
}