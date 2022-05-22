using PIA_BACKEND_MAGG.DTOs.ParticipanteDTO;
using PIA_BACKEND_MAGG.DTOs.RifasDTO;

namespace PIA_BACKEND_MAGG.DTOs.ParticipanteRifaDTO
{
    public class GetParticipanteRifaDTO
    {
        public int id { get; set; }
        public int rifaId { get; set; }
        public ParticipantesDTO participaciones { get; set; }
        public int NumeroLoteria { get; set; }
        public Boolean ganador { get; set; }

    }
}
