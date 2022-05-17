using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PIA_BACKEND_MAGG.DTOs;
using PIA_BACKEND_MAGG.Entidades;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PIA_BACKEND_MAGG.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IConfiguration _config;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IMapper mapper;

        public UsuariosController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager, 
            IConfiguration _config,
            SignInManager<IdentityUser> signInManager,
            IMapper mapper)
        {
            this.context = context;
            this.userManager = userManager;
            this._config = _config;
            this.signInManager = signInManager;
            this.mapper = mapper;
        }

        [HttpGet("Autorizado")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Get()
        {
            return Ok("Esta autorizado");
        }
        
        [HttpPost("registrar")]
        public async Task<IActionResult> registrar(UsuarioCreacionDTO nuevoUSer)
        {

            
            var usuario = new IdentityUser {UserName = nuevoUSer.UserName, Email = nuevoUSer.Email, PhoneNumber = nuevoUSer.PhoneNumber};
            var result = await userManager.CreateAsync(usuario, nuevoUSer.Password);
            var userId = await userManager.GetUserIdAsync(usuario);

            if (result.Succeeded)
            {
                var user = mapper.Map<Participantes>(nuevoUSer);
                user.IdUser = userId;

                context.Add(user);
                await context.SaveChangesAsync();

                // TO DO
                // OBTENER INFORMACION DEL USUARIO EN OTRO ENDPOINTS

                var token = await Generate(mapper.Map<loginUsuarioDTO>(nuevoUSer));
                return Ok("Token: "+token);
            }

            else { return BadRequest(result.Errors); }
        }

        [HttpPost("login")]
        public async Task<ActionResult> iniciarSesion(loginUsuarioDTO user)
        {
            var result = await signInManager.PasswordSignInAsync(
                user.UserName, user.Password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var token = await Generate(user);
                return Ok("Token: " + token);

            }
            else
            {
                return BadRequest("Operacion invalida, intente de nuevo");
            }

        }

        private async Task<string> Generate(loginUsuarioDTO user)
        {

            var usuario = await userManager.FindByNameAsync(user.UserName);

            var claims = new List<Claim> {
                new Claim("UserName", user.UserName),
            };

            var ClaimsDb = await userManager.GetClaimsAsync(usuario);
            claims.AddRange(ClaimsDb);

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credenciales = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
              null,
              null,
              claims,
              expires: DateTime.Now.AddYears(1),
              signingCredentials: credenciales
              );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        //[HttpPost("haceradmin")]
        //public async Task<IActionResult> post(string username)
        //{
        //    var usuario = await userManager.FindByNameAsync(username);
        //    await userManager.AddClaimAsync(usuario, new Claim("administrador", "true"));
        //    return Ok();
        //}

    }
}
