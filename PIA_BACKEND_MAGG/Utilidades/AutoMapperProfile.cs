using AutoMapper;
using Microsoft.AspNetCore.Identity;
using PIA_BACKEND_MAGG.DTOs;
using PIA_BACKEND_MAGG.DTOs.ParticipanteDTO;
using PIA_BACKEND_MAGG.DTOs.ParticipanteRifaDTO;
using PIA_BACKEND_MAGG.DTOs.PremiosDTO;
using PIA_BACKEND_MAGG.DTOs.RifasDTO;
using PIA_BACKEND_MAGG.DTOs.TarjetaGanadoraDTO;
using PIA_BACKEND_MAGG.DTOs.UsuarioDTO;
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
            CreateMap<CreacionParticipanteDTO, Participantes>();
            CreateMap<IdentityUser, IdentityUserDTO>();
            CreateMap<Rifa, GetRifaDTOConParticipantes>()
                .ForMember(rifaDTO => rifaDTO.participantes, opc => opc.MapFrom(MapGetRifaDTOParticipantes));

            //Rifas
            CreateMap<RifaCreacionDTO, RifaDTO>();
            CreateMap<RifaDTO, Rifa>();
            CreateMap<Rifa, GetRifaDTO>();
            CreateMap<RifaCreacionDTO, Rifa>();
            CreateMap<Rifa, PremiosDTOConRifas>()
                .ForMember(rifaDTO => rifaDTO.premios, opc => opc.MapFrom(MapPremiosDTORifa));
            CreateMap<participanteRifaDTO, ParticipanteRifa>();
            CreateMap<ParticipanteRifa, GetParticipanteRifaDTO>()
                .ForMember(participanteRifa => participanteRifa.participaciones, opc => opc.MapFrom(MapGetParicipanteRifaDTO));
            CreateMap<RifaPatchDTO, Rifa>().ReverseMap();

            //Premios
            CreateMap<PremioCreacionDTO, PremioDTO>();
            CreateMap<PremioDTO, Premio>().ReverseMap();
            CreateMap<Premio, GetPremioDTO>().ReverseMap();
            CreateMap<Premio, PremioDTOGanador>();
            CreateMap<PremioCreacionDTO, Premio>();
            CreateMap<UpdatePremioDTO, Premio>();
            CreateMap<PremioPatchDTO, Premio>().ReverseMap();

            //TarjetaGanadora
            CreateMap<TarjetaGanadorDTO, TarjetaGanadora>().ReverseMap();
            CreateMap<TarjetaGanadora, TarjetaGanadorDTO>()
                .ForMember(tarjeta => tarjeta.user, opc => opc.MapFrom(MapTarjetaGanadoraDTO));
            CreateMap<TarjetaGanadorDTO, GetTarjetaGanadoraDTO>()
                .ForMember(tarjeta => tarjeta.user, opc => opc.MapFrom(MapGetTarjetaGanadoraDTO));
        }

        private GetIdentityUserDTO MapGetTarjetaGanadoraDTO(TarjetaGanadorDTO tarjetaGanadorDTO, GetTarjetaGanadoraDTO getTarjetaGanadoraDTO)
        {
            var result = new GetIdentityUserDTO();

            if (tarjetaGanadorDTO.user == null) return result;

            result = new GetIdentityUserDTO()
            {
                nombre = tarjetaGanadorDTO.user.UserName
            };
            return result;
        }

        private ParticipantesDTO MapGetParicipanteRifaDTO(ParticipanteRifa participanteRifa, GetParticipanteRifaDTO getParticipanteRifaDTO)
        {
            var result = new ParticipantesDTO();

            if (result == null) return result;

            result = new ParticipantesDTO()
            {
                nombre = participanteRifa.participante.UserName
            };
            return result;
        }

        private List<ParticipantesDTO> MapGetRifaDTOParticipantes(Rifa rifa, GetRifaDTOConParticipantes rifaParticipantes)
        {
            var result = new List<ParticipantesDTO>();

            if (rifa.participaciones == null) return result;

            foreach (var participante in rifa.participaciones)
            {
                result.Add(new ParticipantesDTO()
                {
                    nombre = participante.participante.UserName
                });
            }
            return result;
        }

        private IdentityUserDTO MapTarjetaGanadoraDTO(TarjetaGanadora tarjetaGanadora, TarjetaGanadorDTO tarjeta)
        {
            var result = new IdentityUserDTO();

            if (tarjetaGanadora.user == null) return result;

            result = new IdentityUserDTO()
            {
                UserName = tarjetaGanadora.user.UserName,
                Email = tarjetaGanadora.user.Email,
                PhoneNumber = tarjetaGanadora.user.PhoneNumber
            };
            return result;
        }

        private List<GetPremioDTO> MapPremiosDTORifa(Rifa rifa, GetRifaDTO getRifaDTO)
        {
            var result = new List<GetPremioDTO>();

            if(rifa.premios == null) return result;

            foreach(var premio in rifa.premios)
            {
                result.Add(new GetPremioDTO()
                {
                    Id = premio.Id,
                    Nombre = premio.Nombre,
                    descripcion = premio.descripcion,
                    disponible = premio.disponible,
                    orden = premio.orden
                });
            }

            return result;
        }
    }
}
