using AutoMapper;
using PIA_BACKEND_MAGG.DTOs;
using PIA_BACKEND_MAGG.DTOs.RifasDTO;
using PIA_BACKEND_MAGG.Entidades;

namespace PIA_BACKEND_MAGG.Utilidades
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<UsuarioCreacionDTO, Participantes>();
            CreateMap<UsuarioCreacionDTO, loginUsuarioDTO>();
            CreateMap<RifaCreacionDTO, RifaDTO>();
            CreateMap<RifaDTO, Rifa>();
        }
    }
}
