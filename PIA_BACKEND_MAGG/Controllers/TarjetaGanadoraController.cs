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

namespace PIA_BACKEND_MAGG.Controllers
{
    [Route("api/[controller]/Rifa/{idRifa:int}")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TarjetaGanadoraController : ControllerBase
    {

        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly UserManager<IdentityUser> userManager;

        public TarjetaGanadoraController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
            this.userManager = userManager;
        }


        [HttpGet("elegirUnGanador")]
        [Authorize(Policy = "Administrador")]
        public async Task<ActionResult<TarjetaGanadorDTO>> GetGanador(int idRifa)
        {
            var rifa = await context.rifas.Where(rifa => rifa.Id == idRifa).FirstOrDefaultAsync();

            if (rifa == null) { return BadRequest("La rifa no existe"); }
            if (rifa.finalizada) { return BadRequest("La rifa ha finalizado"); }

            var participaciones = await context.participantesRifa.Where(x => x.rifaId == idRifa).ToListAsync();

            if (participaciones.Count == 0) return BadRequest("Aun no hay ninguna participacion para esta rifa!");

            var premios = await context.premios.Where(x => x.rifaId == idRifa && x.disponible == true).ToListAsync();

            if (premios.Count == 0) return BadRequest("No hay ningun premio para esta rifa");

            Random rnd = new Random();

            var ganadorRnd = participaciones.OrderBy(x => rnd.Next()).Take(1).FirstOrDefault();
            var premioGanado = premios.Last();

            var userGanador = await context.participantes.Where(x => x.Id == ganadorRnd.participanteId).FirstOrDefaultAsync();
            var user = await userManager.FindByIdAsync(userGanador.IdUser);

            var premioDTOGanador = mapper.Map<PremioDTOGanador>(premioGanado);

            var tarjetaGanadora = new TarjetaGanadora
            {
                nombreRifa = rifa.Nombre,
                user = user,
            };

            ganadorRnd.ganador = true;
            premioGanado.disponible = false;

            context.participantesRifa.Update(ganadorRnd);
            context.premios.Update(premioGanado);
            context.TarjetasGanadoras.Add(tarjetaGanadora);

            await context.SaveChangesAsync();

            premios = await context.premios.Where(x => x.rifaId == idRifa && x.disponible == true).ToListAsync();

            if (premios.Count == 0)
            {
                rifa.finalizada = true;
                context.rifas.Update(rifa);
                await context.SaveChangesAsync();
            }

            var tarjetaGanadoraDTO = mapper.Map<TarjetaGanadorDTO>(tarjetaGanadora);

            return tarjetaGanadoraDTO;
        }
    }
}
