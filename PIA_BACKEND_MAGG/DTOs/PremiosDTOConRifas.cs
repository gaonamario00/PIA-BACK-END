using PIA_BACKEND_MAGG.DTOs.PremiosDTO;
using PIA_BACKEND_MAGG.DTOs.RifasDTO;

namespace PIA_BACKEND_MAGG.DTOs
{
    public class PremiosDTOConRifas: GetRifaDTO
    {
        public List<GetPremioDTO> premios { get; set; }
    }
}
