using PIA_BACKEND_MAGG.Entidades;
using PIA_BACKEND_MAGG.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace PIA_BACKEND_MAGG.DTOs.RifasDTO
{
    public class RifaCreacionDTO
    {
        [Required(ErrorMessage = "El campo de {0} es obligatorio")]
        [RifaNameFormat]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "El campo de {0} es obligatorio")]
        [Range(0,200)]
        public double costo { get; set; }
    }
}
