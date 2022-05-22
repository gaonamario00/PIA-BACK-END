using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PIA_BACKEND_MAGG.DTOs;
using PIA_BACKEND_MAGG.DTOs.RifasDTO;
using PIA_BACKEND_MAGG.Entidades;
using PIA_BACKEND_MAGG.Servicios;

namespace PIA_BACKEND_MAGG.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RifaController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IMapper mapper;
        private readonly ILogger<RifaController> logger;

        public RifaController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            IMapper mapper,
            ILogger<RifaController> logger)
        {
            this.dbContext = context;
            this.userManager = userManager;
            this.mapper = mapper;
            this.logger = logger;
        }

        // Obtiene todas las rifas
        [HttpGet("ConsultarTodas")]
        public async Task<ActionResult<List<GetRifaDTO>>> GetAll()
        {
            var rifas = await dbContext.rifas.ToListAsync();
            return mapper.Map<List<GetRifaDTO>>(rifas);
        }

        //Obtiene todas las rifas con la propiedad finalizada = false.
        [HttpGet("ConsultarVigentes")]
        public async Task<ActionResult<List<GetRifaDTO>>> GetAllVigentes()
        {
            var rifas = await dbContext.rifas.Where(state => state.finalizada == false).ToListAsync();
            return mapper.Map<List<GetRifaDTO>>(rifas);
        }

        //Obtiene una rifa por id con sus premios
        [HttpGet("{idRifa:int}", Name = "consultarRifa")]
        public async Task<ActionResult<PremiosDTOConRifas>> GetById(int idRifa)
        {
            //Obtiene la rifa de la DB con sis premios
            var rifa = await dbContext.rifas.
                Include(rifaDB => rifaDB.premios)
                .FirstOrDefaultAsync(x => x.Id == idRifa);

            if (rifa == null) return NotFound("La rifa ingresada no se encontro");

            return mapper.Map<PremiosDTOConRifas>(rifa);

        }

        //Obtiene las rifas que contengan en su nombre la string enviada por parametro
        [HttpGet("{nombre}")]
        public async Task<ActionResult<List<GetRifaDTO>>> GetByNombre(string nombre)
        {
            var rifas = await dbContext.rifas.Where(x => x.Nombre.Contains(nombre)).ToListAsync();

            if (rifas.Count == 0) return NotFound("No se encontraron resultados");

            return mapper.Map<List<GetRifaDTO>>(rifas);

        }

        //Agrega una rifa
        [HttpPost("Agregar")]
        [Authorize(Policy = "Administrador")]
        public async Task<ActionResult> Post([FromBody] RifaCreacionDTO rifaCreacionDTO)
        {
            //Verifica si existe una rifa con el mismo nombre
            var existeRifa = await dbContext.rifas.AnyAsync(rifa => rifa.Nombre.Equals(rifaCreacionDTO.Nombre) && rifa.finalizada == false);
            if (existeRifa)
            {
                return BadRequest("Ya hay una rifa con ese nombre vigente");
            }

            //Obtiene los datos del usuario
            var userNameClaim = HttpContext.User.Claims.Where(claim => claim.Type == "UserName").FirstOrDefault();
            var userName = userNameClaim.Value;

            var user = await userManager.FindByNameAsync(userName);

            //Setea valores de la rifa
            var rifaDTO = mapper.Map<RifaDTO>(rifaCreacionDTO);
            rifaDTO.userId = user.Id;
            rifaDTO.user = user;
            rifaDTO.finalizada = false;

            var rifa = mapper.Map<Rifa>(rifaDTO);

            //La agrega a la DB
            dbContext.Add(rifa);
            await dbContext.SaveChangesAsync();
            var getRifaDTO = mapper.Map<GetRifaDTO>(rifa);

            logger.LogInformation("Rifa Agregada");
            EscribirEnArchivoMsg.DoWork("Se ha agregado una rifa", "Acciones");

            return CreatedAtRoute("consultarRifa", new { idRifa = getRifaDTO.Id }, getRifaDTO);
        }

        //Actualiza una rifa
        [HttpPut("{idRifa:int}")]
        [Authorize(Policy = "Administrador")]
        public async Task<ActionResult> actualizarRifa(RifaCreacionDTO rifaCreacionDTO, int idRifa)
        {
            // Obtiene la rifa de la DB con user, participaciones y premios
            var rifaDB = await dbContext.rifas
                .Include(rifa => rifa.user)
                .Include(rifa => rifa.participaciones)
                .Include(rifa => rifa.premios)
                .FirstOrDefaultAsync(rifa => rifa.Id == idRifa);

            if (rifaDB == null) return NotFound("No se encontro la rifa");

            //Actualiza el registro
            rifaDB = mapper.Map(rifaCreacionDTO, rifaDB);

            dbContext.Update(rifaDB);
            await dbContext.SaveChangesAsync();

            logger.LogInformation("Rifa con id " + idRifa + " actualizada");
            EscribirEnArchivoMsg.DoWork("Rifa con id " + idRifa + " actualizada", "Acciones");

            return NoContent();
        }

        //Parchea una rifa de la DB
        [HttpPatch("{idRifa:int}")]
        [Authorize(Policy = "Administrador")]
        public async Task<ActionResult> Patch(int idRifa, JsonPatchDocument patchDocument)
        {
            if (patchDocument == null) return BadRequest();

            var rifaDB = await dbContext.rifas.FirstOrDefaultAsync(rifa => rifa.Id == idRifa);

            if (rifaDB == null) return NotFound();

            var rifaDTO = mapper.Map<RifaPatchDTO>(rifaDB);

            patchDocument.ApplyTo(rifaDTO);

            var isValid = TryValidateModel(rifaDTO);

            if(!isValid) return BadRequest(ModelState);

            mapper.Map(rifaDTO, rifaDB);

            await dbContext.SaveChangesAsync();

            logger.LogInformation("Rifa con id "+idRifa+" actualizada desde patch");
            EscribirEnArchivoMsg.DoWork("Rifa con id " + idRifa + " actualizada desde patch", "Acciones");

            return NoContent();

        }

        // Elimina una rifa - elimina tambien premios, participaciones y tarjetas relacionadas.
        [HttpDelete("{idRifa:int}")]
        [Authorize(Policy = "Administrador")]
        public async Task<ActionResult> EliminarRifa(int idRifa)
        {
            var exist = await dbContext.rifas.AnyAsync(x => x.Id == idRifa);
            if (!exist) return NotFound("No se encontro la rifa");

            logger.LogWarning("La rifa con id " + idRifa + " sera eliminada");

            dbContext.Remove(new Rifa
            {
                Id = idRifa
            });

            await dbContext.SaveChangesAsync();

            logger.LogInformation("Rifa con id " + idRifa + " eliminada");
            EscribirEnArchivoMsg.DoWork("Rifa con id " + idRifa + " eliminada", "Acciones");

            return Ok();
        }
    }
}
