using PIA_BACKEND_MAGG.DTOs.PremiosDTO;
using PIA_BACKEND_MAGG.DTOs.UsuarioDTO;

namespace PIA_BACKEND_MAGG.DTOs
{
    public class TarjetaGanadorDTO
    {
        public int id { get; set; }
        public string nombreRifa { get; set; }
        public IdentityUserDTO user { get; set; }
    }
}
