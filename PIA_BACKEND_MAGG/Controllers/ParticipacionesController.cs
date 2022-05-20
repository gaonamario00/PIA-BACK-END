using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PIA_BACKEND_MAGG.DTOs;
using PIA_BACKEND_MAGG.DTOs.ParticipanteRifaDTO;
using PIA_BACKEND_MAGG.Entidades;

namespace PIA_BACKEND_MAGG.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Administrador")]
    public class ParticipacionesController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IMapper mapper;

        public ParticipacionesController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            IMapper mapper)
        {
            this.context = context;
            this.userManager = userManager;
            this.mapper = mapper;
        }

        [HttpGet("Rifa/{idRifa:int}")]
        public async Task<ActionResult<GetRifaDTOConParticipantes>> participantesPorRifa(int idRifa)
        {
            var rifa = await context.rifas
                .Include(rifaDB => rifaDB.participaciones)
                .ThenInclude(participaciones => participaciones.participante)
                .FirstOrDefaultAsync(rifaDB => rifaDB.Id == idRifa);

            if (rifa == null) return NotFound();

            return mapper.Map<GetRifaDTOConParticipantes>(rifa);
        }

        [HttpGet("ConsultarTodas")]
        public async Task<ActionResult<List<GetParticipanteRifaDTO>>> participaciones()
        {
            var participacionesDB = await context.participantesRifa
                .Include(parRifDB => parRifDB.participante)
                .ToListAsync();

            if (participacionesDB.Count == 0) return BadRequest("No se encontraron registros");

            return mapper.Map<List<GetParticipanteRifaDTO>>(participacionesDB);
        }

        [HttpDelete("{idRifa:int}")]
        public async Task<ActionResult> eliminarParticipacion(int idParticipacion)
        {
            var exist = await context.participantesRifa.AnyAsync(x => x.id == idParticipacion);
            if (!exist) return NotFound("No se encontro la participacion");

            context.Remove(new ParticipanteRifa
            {
                id = idParticipacion
            });

            await context.SaveChangesAsync();

            return Ok();
        }

    }
}
