using PIA_BACKEND_MAGG.DTOs.PremiosDTO;
using PIA_BACKEND_MAGG.Entidades;

namespace PIA_BACKEND_MAGG.DTOs.RifasDTO
{
    public class GetRifaDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public double costo { get; set; }
        public Boolean finalizada { get; set; }
    }
}
