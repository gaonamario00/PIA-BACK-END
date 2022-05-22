using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PIA_BACKEND_MAGG.DTOs;

namespace PIA_BACKEND_MAGG.Utilidades
{
    public interface IService
    {
        Guid GetSingleton();
        Task getTarjetas();
    }

    public class Servicio : IService
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly ServiceSingleton serviceSingleton;

        public Servicio(ServiceSingleton serviceSingleton)
        {
            this.context = context;
            this.mapper = mapper;
            this.serviceSingleton = serviceSingleton;
        }

        public List<TarjetaGanadorDTO> GetSingleton()
        {
            return serviceSingleton.tarjetasGanadoras;
        }

        public async Task getTarjetas()
        {
        //    var tarjetas = await dbContext.TarjetasGanadoras.ToListAsync();

        //    var tarjetasDTO = mapper.Map<List<TarjetaGanadorDTO>>(tarjetas);
        //    this.tarjetasGanadoras = tarjetasDTO;
        }

        Guid IService.GetSingleton()
        {
            throw new NotImplementedException();
        }
    }

    public class ServiceSingleton
    {
        public Guid guid = Guid.NewGuid();
        public List<TarjetaGanadorDTO> tarjetasGanadoras = new List<TarjetaGanadorDTO>();


        //public ServiceSingleton(ApplicationDbContext dbContext, IMapper mapper)
        //{
        //    getTarjetas();
        //}
    }
}
