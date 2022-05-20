using PIA_BACKEND_MAGG.Entidades;
using System.ComponentModel.DataAnnotations;

namespace PIA_BACKEND_MAGG.DTOs.PremiosDTO
{
    public class PremioDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string descripcion { get; set; }
        public Boolean disponible { get; set; }
        public int rifaId { get; set; }
        public Rifa rifa { get; set; }
        public int orden { get; set; }
        public double valorado { get; set; }
    }
}
