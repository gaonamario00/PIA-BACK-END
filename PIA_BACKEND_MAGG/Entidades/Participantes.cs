using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace PIA_BACKEND_MAGG.Entidades
{
    public class Participantes
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo de {0} es obligatorio")]
        //CREO QUE ESTO NO VA USERNAME
        public string UserName { get; set; }
        [Required(ErrorMessage = "El campo de {0} es obligatorio")]
        public string IdUser { get; set; }
        public IdentityUser user { get; set; }
        public List<ParticipanteRifa> Participaciones { get; set; }
    }
}
