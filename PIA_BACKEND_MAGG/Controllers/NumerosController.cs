using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PIA_BACKEND_MAGG.DTOs;
using PIA_BACKEND_MAGG.DTOs.ParticipanteRifaDTO;
using PIA_BACKEND_MAGG.DTOs.TarjetaGanadoraDTO;
using PIA_BACKEND_MAGG.Entidades;
using PIA_BACKEND_MAGG.Utilidades;

namespace PIA_BACKEND_MAGG.Controllers
{
    [Route("api/[controller]/Rifa/{idRifa:int}")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class NumerosController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IMapper mapper;

        public NumerosController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            IMapper mapper)
        {
            this.context = context;
            this.userManager = userManager;
            this.mapper = mapper;
        }

        [HttpGet("NumerosDisponibles")]
        public async Task<ActionResult<List<int>>> GetNumerosDisponiblesById(int idRifa)
        {

            var rifaExiste = await context.rifas.AnyAsync(x => x.Id == idRifa);

            if (!rifaExiste) return BadRequest("La rifa ingresada no existe");

            var participacionesPorRifa = await context.participantesRifa.Where(part => part.rifaId == idRifa).ToListAsync();

            if (participacionesPorRifa == null) { return BadRequest("No se encontraron participaciones"); }

            List<int> numerosOcupados = new List<int>();

            foreach (var p in participacionesPorRifa)
            {
                numerosOcupados.Add(p.NumeroLoteria);
            }

            var numerosDisponibles = NumerosRifaConstantes.NumerosRifa.Except(numerosOcupados).ToList();

            return numerosDisponibles;
        }

        [HttpGet("numero/{numeroLoteria:int}", Name = "consultarNumeroLoteria")]
        public async Task<ActionResult<NumeroDTO>> GetNumeroByRifaId(int idRifa, int numeroLoteria)
        {

            if (numeroLoteria < 1 || numeroLoteria > 54)
            {
                return BadRequest("El numero de loteria debe de ser de 1 hasta 54");
            }

            var rifaExiste = await context.rifas.AnyAsync(x => x.Id == idRifa);

            if (!rifaExiste) return BadRequest("La rifa ingresada no existe");

            var number = await context.participantesRifa.AnyAsync(part => part.rifaId == idRifa && part.NumeroLoteria == numeroLoteria);

            var numero = new NumeroDTO { numero = numeroLoteria, estado = null };

            if (number)
            {
                numero.estado = "Ocupado";
            }
            else numero.estado = "Disponible";

            return numero;
        }


        [HttpPost("participar/{numero:int}")]
        public async Task<ActionResult> participar(int idRifa, int numero)
        {
            if (numero < 1 || numero > 54)
            {
                return BadRequest("El numero a elejir debe de ser de 1 hasta 54");
            }

            var numeroOcupado = await context.participantesRifa.AnyAsync(part => part.rifaId == idRifa && part.NumeroLoteria == numero);

            if (numeroOcupado) { return BadRequest("Numero ocupado"); }

            // Obtiene la rifa por id
            var rifa = await context.rifas.Where(rifa => rifa.Id == idRifa).FirstOrDefaultAsync();

            if (rifa == null) return BadRequest("La rifa no existe");
            if (rifa.finalizada) return BadRequest("La rifa ya ha sido finalizada");

            // Obtiene el usuario por username
            var userNameClaim = HttpContext.User.Claims.Where(claim => claim.Type == "UserName").FirstOrDefault();
            var userName = userNameClaim.Value;

            var user = await userManager.FindByNameAsync(userName);

            if (user.Id.Equals(rifa.userId)) { return BadRequest("No puede participar en su propia rifa"); }

            var participanteExiste = await context.participantes.AnyAsync(x => x.IdUser == user.Id);

            if (!participanteExiste)
            {
                var nuevoParticipante = new CreacionParticipanteDTO
                {
                    UserName = userName,
                    IdUser = user.Id,
                    user = user
                };

                var Newparticipante = mapper.Map<Participantes>(nuevoParticipante);

                context.participantes.Add(Newparticipante);
                await context.SaveChangesAsync();

                return NoContent();

            }

            //obtiene el participante por userId
            var participante = await context.participantes.Where(part => part.IdUser == user.Id).FirstOrDefaultAsync();

            //crea la participacion
            var participacionDTO = new participanteRifaDTO
            {
                participanteId = participante.Id,
                participante = participante,
                rifaId = rifa.Id,
                rifa = rifa,
                NumeroLoteria = numero,
                ganador = false
            };


            var participacion = mapper.Map<ParticipanteRifa>(participacionDTO);

            context.participantesRifa.Add(participacion);
            await context.SaveChangesAsync();

            var numeroDTO = new NumeroDTO { numero = numero, estado = "Elegido" };

            return CreatedAtRoute("consultarNumeroLoteria", new { idRifa = idRifa, numeroLoteria = numero }, numeroDTO);
        }


    }
}
