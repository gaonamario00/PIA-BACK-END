using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace PIA_BACKEND_MAGG.DTOs.RifasDTO
{
    public class RifaDTO
    {
        [Required(ErrorMessage = "El campo de {0} es obligatorio")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "El campo de {0} es obligatorio")]
        public double costo { get; set; }
        [Required(ErrorMessage = "El campo de {0} es obligatorio")]
        public string userId { get; set; }
        public IdentityUser user { get; set; }
        public Boolean finalizada { get; set; }
    }
}
