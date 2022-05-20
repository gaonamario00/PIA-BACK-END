using PIA_BACKEND_MAGG.DTOs.ParticipanteDTO;
using PIA_BACKEND_MAGG.DTOs.RifasDTO;

namespace PIA_BACKEND_MAGG.DTOs
{
    public class GetRifaDTOConParticipantes: GetRifaDTO
    {
        public List<ParticipantesDTO> participantes { get; set; }
    }
}
