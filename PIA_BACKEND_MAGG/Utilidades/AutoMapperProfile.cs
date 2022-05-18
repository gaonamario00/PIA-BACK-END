using AutoMapper;
using PIA_BACKEND_MAGG.DTOs;
using PIA_BACKEND_MAGG.DTOs.ParticipanteRifaDTO;
using PIA_BACKEND_MAGG.DTOs.PremiosDTO;
using PIA_BACKEND_MAGG.DTOs.RifasDTO;
using PIA_BACKEND_MAGG.Entidades;

namespace PIA_BACKEND_MAGG.Utilidades
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile()
        {
            //Usuario
            CreateMap<UsuarioCreacionDTO, Participantes>();
            CreateMap<UsuarioCreacionDTO, loginUsuarioDTO>();

            //Rifas
            CreateMap<RifaCreacionDTO, RifaDTO>();
            CreateMap<RifaDTO, Rifa>();
            CreateMap<Rifa, GetRifaDTO>();
            CreateMap<participanteRifaDTO, ParticipanteRifa>();

            //Premios
            CreateMap<PremioCreacionDTO, PremioDTO>();
            CreateMap<PremioDTO, Premio>();
            CreateMap<Premio, GetPremioDTO>();
        }
    }
}
