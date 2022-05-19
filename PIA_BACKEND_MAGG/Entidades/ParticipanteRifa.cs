using System.ComponentModel.DataAnnotations;

namespace PIA_BACKEND_MAGG.Entidades
{
    public class ParticipanteRifa
    {
        public int id { get; set; }
        [Required(ErrorMessage = "El campo de {0} es obligatorio")]
        public int participanteId { get; set; }
        public Participantes participante { get; set; }
        [Required(ErrorMessage = "El campo de {0} es obligatorio")]
        public int rifaId { get; set; }
        public Rifa rifa { get; set; }
        [Required(ErrorMessage = "El campo de {0} es obligatorio")]
        [Range(1,54)]
        public int NumeroLoteria { get; set; }
        [Required(ErrorMessage = "El campo de {0} es obligatorio")]
        public Boolean ganador { get; set; }
        public int? premioId { get; set; }
    }
}
