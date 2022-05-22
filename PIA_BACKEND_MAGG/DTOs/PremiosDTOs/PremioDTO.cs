using PIA_BACKEND_MAGG.Entidades;
using System.ComponentModel.DataAnnotations;

namespace PIA_BACKEND_MAGG.DTOs.PremiosDTO
{
    public class PremioDTO
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo de {0} es obligatorio")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "El campo de {0} es obligatorio")]
        public string descripcion { get; set; }
        public Boolean disponible { get; set; }
        [Required(ErrorMessage = "El campo de {0} es obligatorio")]
        public int rifaId { get; set; }
        public Rifa rifa { get; set; }
        public int orden { get; set; }
        [Required(ErrorMessage = "El campo de {0} es obligatorio")]
        public double valorado { get; set; }
    }
}
