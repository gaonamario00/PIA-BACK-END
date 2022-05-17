using Microsoft.AspNetCore.Identity;

namespace PIA_BACKEND_MAGG.Entidades
{
    public class Participantes
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string IdUser { get; set; }
        public IdentityUser user { get; set; }
        public List<ParticipanteRifa> Participaciones { get; set; }
    }
}
