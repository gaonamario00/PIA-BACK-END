using System.ComponentModel.DataAnnotations;

namespace PIA_BACKEND_MAGG.DTOs.PremiosDTO
{
    public class PremioCreacionDTO
    {
        [Required]
        public string Nombre { get; set; }
        [Required]
        public string descripcion { get; set; }
        [Required]
        public int rifaId { get; set; }

    }
}
