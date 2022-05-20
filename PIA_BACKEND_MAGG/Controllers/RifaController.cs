using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PIA_BACKEND_MAGG.DTOs;
using PIA_BACKEND_MAGG.DTOs.ParticipanteRifaDTO;
using PIA_BACKEND_MAGG.DTOs.RifasDTO;
using PIA_BACKEND_MAGG.Entidades;
using PIA_BACKEND_MAGG.Utilidades;

namespace PIA_BACKEND_MAGG.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RifaController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IMapper mapper;

        public RifaController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            IMapper mapper)
        {
            this.context = context;
            this.userManager = userManager;
            this.mapper = mapper;
        }


        [HttpGet("ConsultarTodas")]
        public async Task<ActionResult<List<GetRifaDTO>>> GetAll()
        {
            var rifas = await context.rifas.ToListAsync();

            return mapper.Map<List<GetRifaDTO>>(rifas);

        }

        [HttpGet("ConsultarVigentes")]
        public async Task<ActionResult<List<GetRifaDTO>>> GetAllVigentes()
        {
            var rifas = await context.rifas.Where(state => state.finalizada == false).ToListAsync();

            if (rifas.Count == 0) BadRequest("No hay rifas actualmente");

            return mapper.Map<List<GetRifaDTO>>(rifas);

        }

        //Modificar este endpoint para usar INCLUDE y THEINCLUDE
        [HttpGet("{idRifa:int}", Name = "consultarRifa")]
        public async Task<ActionResult<PremiosDTOConRifas>> GetById(int idRifa)
        {
            var rifa = await context.rifas.
                Include(rifaDB => rifaDB.premios)
                .FirstOrDefaultAsync(x => x.Id == idRifa);

            if (rifa == null) return BadRequest("La rifa ingresada no existe");

            return mapper.Map<PremiosDTOConRifas>(rifa);

        }

        [HttpGet("{nombre}")]
        public async Task<ActionResult<List<GetRifaDTO>>> GetByNombre(string nombre)
        {
            var rifas = await context.rifas.Where(x => x.Nombre.Contains(nombre)).ToListAsync();

            if (rifas.Count == 0) return BadRequest("La rifa ingresada no existe");

            return mapper.Map<List<GetRifaDTO>>(rifas);

        }

        //[HttpGet("{idRifa}/NumerosDisponibles")]
        //public async Task<ActionResult<List<int>>> GetNumerosDisponiblesById(int idRifa)
        //{

        //    var rifaExiste = await context.rifas.AnyAsync(x => x.Id == idRifa);

        //    if(!rifaExiste) return BadRequest("La rifa ingresada no existe");

        //    var participacionesPorRifa = await context.participantesRifa.Where(part => part.rifaId == idRifa).ToListAsync();

        //    if (participacionesPorRifa == null) { return BadRequest("No se encontraron participaciones"); }

        //    List<int> numerosOcupados = new  List<int>();

        //    foreach (var p in participacionesPorRifa)
        //    {
        //        numerosOcupados.Add(p.NumeroLoteria);
        //    }

        //    var numerosDisponibles = NumerosRifaConstantes.NumerosRifa.Except(numerosOcupados).ToList();

        //    return numerosDisponibles;
        //}

        //[HttpGet("{idRifa}/{numeroLoteria}")]
        //public async Task<ActionResult<string>> GetNumeroByRifaId(int idRifa, int numeroLoteria)
        //{

        //    if (numeroLoteria < 1 || numeroLoteria > 54)
        //    {
        //        return BadRequest("El numero de loteria debe de ser de 1 hasta 54");
        //    }

        //    var rifaExiste = await context.rifas.AnyAsync(x => x.Id == idRifa);

        //    if (!rifaExiste) return BadRequest("La rifa ingresada no existe");

        //    var number = await context.participantesRifa.AnyAsync(part => part.rifaId == idRifa && part.NumeroLoteria == numeroLoteria);

        //    //var numero = new NumeroDTO { number = numeroLoteria, estado = null };

        //    string estado = "";

        //    if (number)
        //    {
        //        estado = "Ocupado";
        //    }else estado = "Disponible";

        //    return estado;
        //}

        [HttpPost("Agregar")]
        [Authorize(Policy = "Administrador")]
        public async Task<IActionResult> Post(RifaCreacionDTO rifaCreacionDTO)
        {

            var existeRifa = await context.rifas.AnyAsync(rifa => rifa.Nombre == rifaCreacionDTO.Nombre && rifa.finalizada == false);
            if (existeRifa)
            {
                return BadRequest("Ya hay una rifa con ese nombre vigente");
            }

            var userNameClaim = HttpContext.User.Claims.Where(claim => claim.Type == "UserName").FirstOrDefault();
            var userName = userNameClaim.Value;

            var user = await userManager.FindByNameAsync(userName);

            var rifaDTO = mapper.Map<RifaDTO>(rifaCreacionDTO);
            rifaDTO.userId = user.Id;
            rifaDTO.user = user;
            rifaDTO.finalizada = false;

            var rifa = mapper.Map<Rifa>(rifaDTO);

            context.Add(rifa);
            await context.SaveChangesAsync();

            var getRifaDTO = mapper.Map<GetRifaDTO>(rifa);

            return CreatedAtRoute("consultarRifa", new { idRifa = getRifaDTO.Id }, getRifaDTO);
        }

        [HttpPut("{idRifa:int}")]
        [Authorize(Policy = "Administrador")]
        public async Task<ActionResult> actualizarRifa(RifaCreacionDTO rifaCreacionDTO, int idRifa)
        {
            var rifaDB = await context.rifas
                .Include(rifa => rifa.user)
                .Include(rifa => rifa.participaciones)
                .Include(rifa => rifa.premios)
                .FirstOrDefaultAsync(rifa => rifa.Id == idRifa);

            if (rifaDB == null) return NotFound("No se encontro la rifa");

            rifaDB = mapper.Map(rifaCreacionDTO, rifaDB);

            context.Update(rifaDB);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{idRifa:int}")]
        [Authorize(Policy = "Administrador")]
        public async Task<ActionResult> Patch(int idRifa, JsonPatchDocument patchDocument)
        {
            if (patchDocument == null) return BadRequest();

            var rifaDB = await context.rifas.FirstOrDefaultAsync(rifa => rifa.Id == idRifa);

            if (rifaDB == null) return NotFound();

            var rifaDTO = mapper.Map<RifaPatchDTO>(rifaDB);

            patchDocument.ApplyTo(rifaDTO);

            var isValid = TryValidateModel(rifaDTO);

            if(!isValid) return BadRequest(ModelState);

            mapper.Map(rifaDTO, rifaDB);

            await context.SaveChangesAsync();
            return NoContent();

        }

        [HttpDelete("{idRifa:int}")]
        [Authorize(Policy = "Administrador")]
        public async Task<ActionResult> EliminarRifa(int idRifa)
        {
            var exist = await context.rifas.AnyAsync(x => x.Id == idRifa);
            if (!exist) return NotFound("No se encontro la rifa");

            context.Remove(new Rifa
            {
                Id = idRifa
            });

            await context.SaveChangesAsync();

            return Ok();
        }
    }
}
