using Microsoft.AspNetCore.Identity;

namespace PIA_BACKEND_MAGG.Entidades
{
    public class TarjetaGanadora
    {
        public int id { get; set; }
        public int idRifa { get; set; }
        public Rifa rifa { get; set; }
        public string nombreRifa { get; set; }
        public string userId { get; set; }
        public IdentityUser user { get; set; }
        public int premioId { get; set; }
    }
}
