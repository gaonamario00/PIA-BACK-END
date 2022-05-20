using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PIA_BACKEND_MAGG.DTOs.PremiosDTO;
using PIA_BACKEND_MAGG.Entidades;
using System.Net;

namespace PIA_BACKEND_MAGG.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PremiosController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public PremiosController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet("ConsularTodos")]
        public async Task<ActionResult<List<GetPremioDTO>>> Get()
        {
            var premios = await context.premios.ToListAsync();

            return mapper.Map<List<GetPremioDTO>>(premios);
        }

        [HttpGet("{idPremio:int}", Name = "consultarPremio")]
        public async Task<ActionResult<GetPremioDTO>> GetPremioById(int idPremio)
        {
            var premio = await context.premios.FirstOrDefaultAsync(premio => premio.Id == idPremio);

            if (premio == null)
            {
                return NotFound();
            }

            return mapper.Map<GetPremioDTO>(premio);

        }

        [HttpGet("Rifa/{idRifa:int}")]
        public async Task<ActionResult<List<GetPremioDTO>>> GetPremiosPorRifa(int idRifa)
        {
            var premios = await context.premios.Where(premio => premio.rifaId == idRifa).ToListAsync();

            if(premios == null) { return NotFound("No se encontraton premios"); }

            return mapper.Map<List<GetPremioDTO>>(premios);
        }

        [HttpPost("Agregar")]
        [Authorize(Policy = "Administrador")]
        public async Task<ActionResult> Add(PremioCreacionDTO premioCreacionDTO)
        {
            var rifa = await context.rifas.Where(rifa => rifa.Id == premioCreacionDTO.rifaId).FirstOrDefaultAsync();

            if(rifa == null) { return BadRequest("La rifa ingresada no existe"); }
            if (rifa.finalizada) return BadRequest("La rifa ingresada ha finalizado");

            var premioDTO = mapper.Map<PremioDTO>(premioCreacionDTO);
            premioDTO.disponible = true;
            premioDTO.rifa = rifa;

            var premiosRifa = await context.premios.Where(premio => premio.rifaId == premioDTO.rifaId).ToListAsync();
            premioDTO.orden = premiosRifa.Count + 1;

            var premio = mapper.Map<Premio>(premioDTO);

            premiosRifa.Add(premio);
            rifa.premios = premiosRifa;

            context.rifas.Update(rifa);

            context.premios.Add(premio);
            await context.SaveChangesAsync();

            var getPremioDTO = mapper.Map<GetPremioDTO>(premio);

            return CreatedAtRoute("consultarPremio", new { idPremio = premio.Id }, getPremioDTO);
        }

    }
}
