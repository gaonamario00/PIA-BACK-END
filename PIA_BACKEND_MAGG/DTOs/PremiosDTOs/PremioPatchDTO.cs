using System.ComponentModel.DataAnnotations;

namespace PIA_BACKEND_MAGG.DTOs.PremiosDTO
{
    public class PremioPatchDTO
    {
        [Required(ErrorMessage = "El campo de {0} es obligatorio")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "El campo de {0} es obligatorio")]
        public string descripcion { get; set; }
        [Required(ErrorMessage = "El campo de {0} es obligatorio")]
        public double valorado { get; set; }

    }


}
