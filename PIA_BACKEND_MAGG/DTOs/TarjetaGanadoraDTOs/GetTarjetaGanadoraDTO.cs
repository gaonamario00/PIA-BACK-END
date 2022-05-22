using PIA_BACKEND_MAGG.DTOs.UsuarioDTO;

namespace PIA_BACKEND_MAGG.DTOs.TarjetaGanadoraDTO
{
    public class GetTarjetaGanadoraDTO
    {
        public string nombreRifa { get; set; }
        public GetIdentityUserDTO user { get; set; }
    }
}
