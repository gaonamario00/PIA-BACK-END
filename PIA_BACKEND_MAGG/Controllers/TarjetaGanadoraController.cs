using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PIA_BACKEND_MAGG.DTOs;
using PIA_BACKEND_MAGG.DTOs.PremiosDTO;
using PIA_BACKEND_MAGG.DTOs.UsuarioDTO;
using PIA_BACKEND_MAGG.Entidades;
using PIA_BACKEND_MAGG.Utilidades;

namespace PIA_BACKEND_MAGG.Controllers
{
    [Route("api/[controller]/Rifa/{idRifa:int}")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TarjetaGanadoraController : ControllerBase
    {

        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;
        private readonly UserManager<IdentityUser> userManager;
        private readonly ServiceSingleton serviceSingleton;
        private readonly ILogger<RifaController> logger;

        public TarjetaGanadoraController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            IMapper mapper, 
            ServiceSingleton serviceSingleton,
            ILogger<RifaController> logger)
        {
            this.dbContext = context;
            this.mapper = mapper;
            this.userManager = userManager;
            this.serviceSingleton = serviceSingleton;
            this.logger = logger;
        }

        // Permite recorrer las tarjetas ganadoras de manera random por sesion sin repetir
        [HttpGet("/api/TarjetaGanadoras/Recorrido")]
        public async Task<ActionResult<TarjetaGanadorDTO>> getTarjetasSingleton()
        {
            var tarjetas = await dbContext.TarjetasGanadoras
                .Include(tarjetasDB => tarjetasDB.user)
                .ToListAsync();

            if (tarjetas.Count == 0) return BadRequest("No se encontraron tarjetas");

            var tarjetasDTO = mapper.Map<List<TarjetaGanadorDTO>>(tarjetas);

            foreach(var tarjetaSingle in serviceSingleton.tarjetasGanadoras)
            {
                foreach (var tarjetaDTO in tarjetasDTO)
                {
                    if(tarjetaSingle.id == tarjetaDTO.id)
                    {
                        tarjetasDTO.Remove(tarjetaDTO);
                        break;
                    }
                }
            }

            Random rnd = new Random();

            var tarjetaDTORnd = tarjetasDTO.OrderBy(x => rnd.Next()).Take(1).FirstOrDefault();

            if (tarjetaDTORnd == null) return BadRequest("Ya se han mostrado todas las tarjetas");

            serviceSingleton.tarjetasGanadoras.Add(tarjetaDTORnd);

            var getTarjetaGanadorDTO = mapper.Map<TarjetaGanadorDTO>(tarjetaDTORnd);

            return getTarjetaGanadorDTO;
        }

        // Permite elejir un ganador
        [HttpGet("elegirUnGanador")]
        [Authorize(Policy = "Administrador")]
        public async Task<ActionResult<TarjetaGanadorDTO>> GetGanador(int idRifa)
        {
            var rifa = await dbContext.rifas.Where(rifa => rifa.Id == idRifa).FirstOrDefaultAsync();

            if (rifa == null) { return BadRequest("La rifa no existe"); }
            if (rifa.finalizada) { return BadRequest("La rifa ha finalizado"); }

            var participaciones = await dbContext.participantesRifa.Where(x => x.rifaId == idRifa).ToListAsync();

            if (participaciones.Count == 0) return BadRequest("Aun no hay ninguna participacion para esta rifa!");

            var premios = await dbContext.premios.Where(x => x.rifaId == idRifa && x.disponible == true).ToListAsync();

            if (premios.Count == 0) return BadRequest("No hay ningun premio para esta rifa");

            Random rnd = new Random();

            var ganadorRnd = participaciones.OrderBy(x => rnd.Next()).Take(1).FirstOrDefault();
            var premioGanado = premios.Last();

            var userGanador = await dbContext.participantes.Where(x => x.Id == ganadorRnd.participanteId).FirstOrDefaultAsync();
            var user = await userManager.FindByIdAsync(userGanador.IdUser);

            var premioDTOGanador = mapper.Map<PremioDTOGanador>(premioGanado);

            var tarjetaGanadora = new TarjetaGanadora
            {
                rifa = rifa,
                idRifa = rifa.Id,
                nombreRifa = rifa.Nombre,
                user = user,
                premioId = premioGanado.Id
            };

            ganadorRnd.ganador = true;
            premioGanado.disponible = false;

            dbContext.participantesRifa.Update(ganadorRnd);
            dbContext.premios.Update(premioGanado);
            dbContext.TarjetasGanadoras.Add(tarjetaGanadora);

            await dbContext.SaveChangesAsync();

            premios = await dbContext.premios.Where(x => x.rifaId == idRifa && x.disponible == true).ToListAsync();

            if (premios.Count == 0)
            {
                rifa.finalizada = true;
                dbContext.rifas.Update(rifa);
                await dbContext.SaveChangesAsync();
            }

            var tarjetaGanadoraDTO = mapper.Map<TarjetaGanadorDTO>(tarjetaGanadora);

            return tarjetaGanadoraDTO;
        }

        // obtiene todas las tarjetas por rifa
        [HttpGet("ObtenerTodas")]
        public async Task<ActionResult<List<TarjetaGanadorDTO>>> GetTarjetasPorRifa(int idRifa)
        {
            var rifas = await dbContext.rifas.Where(rifa => rifa.Id == idRifa).ToListAsync();

            if (rifas == null) return NotFound("No se encontro la rifa");
            if (rifas.Count == 0) return BadRequest("La rifa aun no tiene tarjetas");

            var tarjetas = await dbContext.TarjetasGanadoras.Where(tarjeta => tarjeta.idRifa == idRifa).ToListAsync();

            if (tarjetas.Count == 0) { return NotFound("No se encontraron tarjetas"); }

            return mapper.Map<List<TarjetaGanadorDTO>>(tarjetas);
        }
    }
}
