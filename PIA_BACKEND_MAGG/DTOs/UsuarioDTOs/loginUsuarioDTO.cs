using System.ComponentModel.DataAnnotations;

namespace PIA_BACKEND_MAGG.DTOs
{
    public class loginUsuarioDTO
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [MinLength(6)]
        public string Password { get; set; }

    }
}
