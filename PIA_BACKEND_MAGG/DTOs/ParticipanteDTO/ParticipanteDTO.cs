using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace PIA_BACKEND_MAGG.DTOs
{
    public class ParticipanteDTO
    {
        public string UserName { get; set; }
        [Required(ErrorMessage = "El campo de {0} es obligatorio")]
        public string IdUser { get; set; }
        public IdentityUser user { get; set; }
    }
}
