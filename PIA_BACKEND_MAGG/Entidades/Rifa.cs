using Microsoft.AspNetCore.Identity;
using PIA_BACKEND_MAGG.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace PIA_BACKEND_MAGG.Entidades
{
    public class Rifa
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo de {0} es obligatorio")]
        [RifaNameFormat]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "El campo de {0} es obligatorio")]
        public double costo { get; set; }
        [Required(ErrorMessage = "El campo de {0} es obligatorio")]
        public string userId { get; set; }
        public IdentityUser user { get; set; } 
        public List<ParticipanteRifa> participaciones { get; set; }
        public List<Premio> premios { get; set; }
        [Required(ErrorMessage = "El campo de {0} es obligatorio")]
        public Boolean finalizada { get; set; }
    }
}
