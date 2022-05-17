namespace PIA_BACKEND_MAGG.Entidades
{
    public class ParticipanteRifa
    {
        public int Id { get; set; }
        public int participanteId { get; set; }
        public Participantes participante { get; set; }
        public int rifaId { get; set; }
        public Rifa rifa { get; set; }
        public int NumeroLoteria { get; set; }
    }
}
