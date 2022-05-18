using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PIA_BACKEND_MAGG.DTOs.ParticipanteRifaDTO;
using PIA_BACKEND_MAGG.DTOs.RifasDTO;
using PIA_BACKEND_MAGG.Entidades;
using PIA_BACKEND_MAGG.Utilidades;
using System.Net;
using System.Security.Claims;

namespace PIA_BACKEND_MAGG.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RifaController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IConfiguration _config;
        private readonly IMapper mapper;

        public RifaController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            IConfiguration _config,
            IMapper mapper)
        {
            this.context = context;
            this.userManager = userManager;
            this._config = _config;
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


            return mapper.Map<List<GetRifaDTO>>(rifas);

        }

        //Modificar este endpoint para usar INCLUDE y THEINCLUDE
        [HttpGet("{idRifa:int}")]
        public async Task<ActionResult<List<GetRifaDTO>>> GetById(int idRifa)
        {
            var rifas = await context.rifas.Where(state => state.Id == idRifa).ToListAsync();

            return mapper.Map<List<GetRifaDTO>>(rifas);

        }

        [HttpGet("{idRifa}/NumerosDisponibles")]
        public async Task<ActionResult<List<int>>> GetNumerosDisponiblesById(int idRifa)
        {
            var participacionesPorRifa = await context.participanteRifa.Where(part => part.rifaId == idRifa).ToListAsync();
            
            if (participacionesPorRifa == null) { return BadRequest("No se encontraron participaciones"); }

            List<int> numerosOcupados = new  List<int>();

            foreach (var p in participacionesPorRifa)
            {
                numerosOcupados.Add(p.NumeroLoteria);
            }

            var numerosDisponibles = NumerosRifaConstantes.NumerosRifa.Except(numerosOcupados).ToList();

            return numerosDisponibles;
        }

        [HttpGet("{idRifa}/{numeroLoteria}")]
        public async Task<ActionResult<NumeroDTO>> GetNumeroByRifaId(int idRifa, int numeroLoteria)
        {
            var number = await context.participanteRifa.AnyAsync(part => part.rifaId == idRifa && part.NumeroLoteria == numeroLoteria);

            var numero = new NumeroDTO { number = numeroLoteria, estado = null };

            if (number)
            {
                numero.estado = "Ocupado";
            }else numero.estado = "Disponible";

            return numero;

        }

        [HttpGet("{idRifa:int}/elegirUnGanador")]
        [Authorize(Policy = "Administrador")]
        public async Task<ActionResult<GanadorDTO>> GetGanador(int idRifa)
        {
            var participaciones = await context.participanteRifa.Where(x => x.rifaId == idRifa).ToListAsync();

            if (participaciones == null) return BadRequest("Aun no hay ninguna participacion para esta rifa!");
            
            var premios = await context.premios.Where(x => x.rifaId == idRifa).ToListAsync();

            if (premios == null) return BadRequest("Aun no hay ningun premio para esta rifa!");

            Boolean premiosAgotados = true;

            foreach(var premio in premios)
            {
                if (premio.disponible) premiosAgotados = false;
            }

            if (premiosAgotados) { return BadRequest("Esta rifa ha terminado"); }

            Random rnd = new Random();

            var ganadorRnd = participaciones.OrderBy(x => rnd.Next()).Take(1).FirstOrDefault();
            var premioGanado = premios.OrderBy(x => rnd.Next()).Take(1).FirstOrDefault();

            var userGanador = await context.participantes.Where(x => x.Id == ganadorRnd.participanteId).FirstOrDefaultAsync();

            var ganadorDTO = new GanadorDTO {
                numero = ganadorRnd.NumeroLoteria, 
                participante = userGanador.UserName,
                premio = premioGanado.Nombre
            };

            ganadorRnd.ganador = true;
            premioGanado.disponible = false;

            context.participanteRifa.Update(ganadorRnd);
            context.premios.Update(premioGanado);

            await context.SaveChangesAsync();

            return ganadorDTO;
        }

        [HttpPost("Agregar")]
        [Authorize(Policy = "Administrador")]
        public async Task<IActionResult> Post(RifaCreacionDTO rifaCreacionDTO)
        {

            var existeRifa = await context.rifas.AnyAsync( rifa => rifa.Nombre == rifaCreacionDTO.Nombre && rifa.finalizada == false);
            if (existeRifa)
            {
                return BadRequest("Ya hay una rifa con ese nombre vigente");
            }

            var userNameClaim = HttpContext.User.Claims.Where(claim=>claim.Type == "UserName").FirstOrDefault();
            var userName = userNameClaim.Value;

            var user = await userManager.FindByNameAsync(userName);

            var rifaDTO = mapper.Map<RifaDTO>(rifaCreacionDTO);
            rifaDTO.userId = user.Id;
            rifaDTO.user = user;
            rifaDTO.finalizada = false;

            var rifa = mapper.Map<Rifa>(rifaDTO);

            context.Add(rifa);
            await context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("{idRifa:int}/participar/{numero:int}")]
        public async Task<ActionResult> participar(int idRifa, int numero)
        {
            if(numero<1 || numero > 54)
            {
                return BadRequest("El numero a elejir debe de ser de 1 hasta 54");
            }

            var numeroOcupado = await context.participanteRifa.AnyAsync(part => part.rifaId == idRifa && part.NumeroLoteria == numero);

            if(numeroOcupado) { return BadRequest("Numero ocupado"); }

            // Obtiene la rifa por id
            var rifa = await context.rifas.Where(rifa => rifa.Id == idRifa).FirstOrDefaultAsync();

            // Obtiene el usuario por username
            var userNameClaim = HttpContext.User.Claims.Where(claim => claim.Type == "UserName").FirstOrDefault();
            var userName = userNameClaim.Value;

            var user = await userManager.FindByNameAsync(userName);
            
            //obtiene el participante por userId
            var participante = await context.participantes.Where(part => part.IdUser == user.Id).FirstOrDefaultAsync();

            //crea la participacion
            var participacionDTO = new participanteRifaDTO { 
                participanteId = participante.Id, 
                participante = participante,
                rifaId = rifa.Id,
                rifa = rifa,
                NumeroLoteria = numero
            };

            var participacion = mapper.Map<ParticipanteRifa>(participacionDTO);

            context.participanteRifa.Add(participacion);
            await context.SaveChangesAsync();

            return Ok();
        }
    }
}
