using PIA_BACKEND_MAGG.DTOs.PremiosDTO;

namespace PIA_BACKEND_MAGG.DTOs.RifasDTO
{
    public class RifaGanadorDTO
    {
        public string Nombre { get; set; }
        public List<PremioRifaDTO> premios { get; set; }
        public string estado { get; set; }
    }
}
