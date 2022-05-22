using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PIA_BACKEND_MAGG.DTOs.PremiosDTO;
using PIA_BACKEND_MAGG.Entidades;
using PIA_BACKEND_MAGG.Servicios;
using System.Net;

namespace PIA_BACKEND_MAGG.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PremiosController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;
        private readonly ILogger<RifaController> logger;

        public PremiosController(ApplicationDbContext context, IMapper mapper, ILogger<RifaController> logger)
        {
            this.dbContext = context;
            this.mapper = mapper;
            this.logger = logger;
        }

        // Obtiene todos los premios
        [HttpGet("ConsularTodos")]
        public async Task<ActionResult<List<GetPremioDTO>>> Get()
        {
            var premios = await dbContext.premios.ToListAsync();

            return mapper.Map<List<GetPremioDTO>>(premios);
        }

        // Obtiene un premio por id
        [HttpGet("{idPremio:int}", Name = "consultarPremio")]
        public async Task<ActionResult<GetPremioDTO>> GetPremioById(int idPremio)
        {
            var premio = await dbContext.premios.FirstOrDefaultAsync(premio => premio.Id == idPremio);

            if (premio == null)
            {
                return NotFound();
            }

            return mapper.Map<GetPremioDTO>(premio);

        }

        //Obtiene todos los premios de una rifa
        [HttpGet("Rifa/{idRifa:int}")]
        public async Task<ActionResult<List<GetPremioDTO>>> GetPremiosPorRifa(int idRifa)
        {
            var existRifa = await dbContext.rifas.AnyAsync(rifa => rifa.Id == idRifa);

            if(!existRifa) return NotFound("No se encontro la rifa");

            var premios = await dbContext.premios.Where(premio => premio.rifaId == idRifa).ToListAsync();

            if(premios == null) { return NotFound("No se encontraton premios"); }

            return mapper.Map<List<GetPremioDTO>>(premios);
        }

        //Agrega un premio a la DB
        [HttpPost("Agregar")]
        [Authorize(Policy = "Administrador")]
        public async Task<ActionResult> Add([FromBody] PremioCreacionDTO premioCreacionDTO)
        {
            var rifa = await dbContext.rifas.Where(rifa => rifa.Id == premioCreacionDTO.rifaId).FirstOrDefaultAsync();
            // Verifica si la rifa existe y si esta vigente
            if(rifa == null) { return NotFound("La rifa ingresada no se encontro"); }
            if (rifa.finalizada) return BadRequest("La rifa ingresada ha finalizado");

            //Verifica si ya existe un nombre con el mismo nombre
            var existeNombrePremio = await dbContext.premios.AnyAsync(premio => premio.Nombre.Equals(premioCreacionDTO.Nombre));
            if (existeNombrePremio) return BadRequest("Ya existe un premio con ese nombre, ingrese otro");

            //Setea los valores del premio
            var premioDTO = mapper.Map<PremioDTO>(premioCreacionDTO);
            premioDTO.disponible = true;
            premioDTO.rifa = rifa;

            var premiosRifa = await dbContext.premios.Where(premio => premio.rifaId == premioDTO.rifaId).ToListAsync();

            //Verifica si se estan obteniendo los premios actualmente
            var rifaYaComenzo = verificarRifacorriendo(premiosRifa);
            if (rifaYaComenzo) return BadRequest("Ya no es posible agregar premios a esta rifa");

            premioDTO.orden = premiosRifa.Count + 1;

            var premio = mapper.Map<Premio>(premioDTO);
            premiosRifa.Add(premio);
            rifa.premios = premiosRifa;

            //Actualiza los premios de la rifa
            dbContext.rifas.Update(rifa);

            //agrega el premio a la DB
            dbContext.premios.Add(premio);
            await dbContext.SaveChangesAsync();

            var getPremioDTO = mapper.Map<GetPremioDTO>(premio);

            return CreatedAtRoute("consultarPremio", new { idPremio = premio.Id }, getPremioDTO);
        }

        //Verifica si todos los premios estan disponibles
        private bool verificarRifacorriendo(List<Premio> premios)
        {
            foreach (var premio in premios)
            {
                if (premio.disponible == false)
                {
                    return true;
                }
            }
            return false;
        }

        //Actualiza un premio
        [HttpPut("{idPremio:int}")]
        [Authorize(Policy = "Administrador")]
        public async Task<ActionResult> actualizarPremio(UpdatePremioDTO updatePremioDTO, int idPremio)
        {
            var premioDB = await dbContext.premios
                .FirstOrDefaultAsync(premio => premio.Id == idPremio);

            if (premioDB == null) return NotFound("No se encontro el premio");

            premioDB = mapper.Map(updatePremioDTO, premioDB);

            dbContext.Update(premioDB);
            await dbContext.SaveChangesAsync();

            logger.LogInformation("Premio con id " + idPremio + " actualizado");
            EscribirEnArchivoMsg.DoWork("Premio con id " + idPremio + " actualizado", "Acciones");

            return NoContent();
        }

        //Parchea un premio
        [HttpPatch("{idPremio:int}")]
        [Authorize(Policy = "Administrador")]
        public async Task<ActionResult> Patch(int idPremio, JsonPatchDocument patchDocument)
        {
            if (patchDocument == null) return BadRequest();

            var premioDB = await dbContext.premios.FirstOrDefaultAsync(premio => premio.Id == idPremio);

            if (premioDB == null) return NotFound();

            var premioDTO = mapper.Map<PremioPatchDTO>(premioDB);

            patchDocument.ApplyTo(premioDTO);

            var isValid = TryValidateModel(premioDTO);

            if (!isValid) return BadRequest(ModelState);

            mapper.Map(premioDTO, premioDB);

            await dbContext.SaveChangesAsync();

            logger.LogInformation("Premio con id " + idPremio + " actualizado desde patch");
            EscribirEnArchivoMsg.DoWork("Premio con id " + idPremio + " actualizado desde patch", "Acciones");

            return NoContent();

        }

        //[HttpDelete("{idPremio:int}")]
        //[Authorize(Policy = "Administrador")]
        //public async Task<ActionResult> EliminarPremio(int idPremio)
        //{
        //    var exist = await dbContext.premios.AnyAsync(x => x.Id == idPremio);
        //    if (!exist) return NotFound("No se encontro el premio");

        //    logger.LogWarning("El premio con id " + idPremio + " sera eliminado");

        //    dbContext.Remove(new Premio
        //    {
        //        Id = idPremio
        //    });


        //    var premioDB = await dbContext.premios.FirstOrDefaultAsync(premio => premio.Id == idPremio);
        //    var rifaId = premioDB.rifaId;

        //    await dbContext.SaveChangesAsync();


        //    var premiosDeLaRifaDB = await dbContext.premios.Where(premio => premio.rifaId == rifaId).ToListAsync();
        //    var i = 0;
        //    foreach (var premios in premiosDeLaRifaDB)
        //    {
        //        //if(idPremio != premios.Id)
        //        //{
        //            premios.orden = i++;
        //            i++;
        //            dbContext.premios.Update(premios);
        //        await dbContext.SaveChangesAsync();
        //        //}
        //    }


        //    logger.LogInformation("Premio con id " + idPremio + " eliminado");
        //    EscribirEnArchivoMsg.DoWork("Premio con id " + idPremio + " eliminado", "Acciones");

        //    return Ok();
        //}

    }
}
