using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PIA_BACKEND_MAGG.DTOs.RifasDTO;
using PIA_BACKEND_MAGG.Entidades;
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


        [HttpGet("ConsultarRifas")]
        public async Task<IActionResult> Get()
        {
            return Ok("Rifas mostradas xd");
        }

        [HttpPost("AgregarRifa")]
        [Authorize(Policy = "Administrador")]
        public async Task<IActionResult> Post(RifaCreacionDTO rifaCreacionDTO)
        {
            var userNameClaim = HttpContext.User.Claims.Where(claim=>claim.Type == "UserName").FirstOrDefault();
            var userName = userNameClaim.Value;

            var user = await userManager.FindByNameAsync(userName);

            var rifaDTO = mapper.Map<RifaDTO>(rifaCreacionDTO);
            rifaDTO.userId = user.Id;

            var rifa = mapper.Map<Rifa>(rifaDTO);
            return Ok("Eres admin, si puedes");
        }
    }
}
