using System.ComponentModel.DataAnnotations;

namespace PIA_BACKEND_MAGG.DTOs
{
    public class UsuarioCreacionDTO
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [EmailAddress]
        public string EmailConfirmed { get; set; }
        [Required]
        [MinLength(6)]
        public string Password { get; set; }
        [Required]
        [MinLength(6)]
        public string PasswordConfirmed { get; set; }
        [Required]
        [StringLength(maximumLength:20, ErrorMessage = "El telefono no puede contener mas de 15 caracteres")]
        public string PhoneNumber { get; set; }
    }
}
