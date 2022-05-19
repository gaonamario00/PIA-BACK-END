using PIA_BACKEND_MAGG.Entidades;

namespace PIA_BACKEND_MAGG.DTOs.ParticipanteRifaDTO
{
    public class participanteRifaDTO
    {
        public int participanteId { get; set; }
        public int rifaId { get; set; }
        public Participantes participante { get; set; }
        public Rifa rifa { get; set; }
        public int NumeroLoteria { get; set; }
        public Boolean ganador { get; set; }
        public int? premioId { get; set; }
    }
}