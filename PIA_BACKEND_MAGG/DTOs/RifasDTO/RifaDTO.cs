using Microsoft.AspNetCore.Identity;

namespace PIA_BACKEND_MAGG.DTOs.RifasDTO
{
    public class RifaDTO
    {
        public string Nombre { get; set; }
        public double costo { get; set; }
        public string userId { get; set; }
        public IdentityUser user { get; set; }
        public Boolean finalizada { get; set; }
    }
}
