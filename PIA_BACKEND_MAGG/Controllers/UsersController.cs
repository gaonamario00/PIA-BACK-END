using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PIA_BACKEND_MAGG.DTOs;
using PIA_BACKEND_MAGG.Utilidades;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PIA_BACKEND_MAGG.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IConfiguration _config;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly ServiceSingleton serviceSingleton;

        public UsersController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager, 
            IConfiguration _config,
            SignInManager<IdentityUser> signInManager,
            IMapper mapper,
            ServiceSingleton serviceSingleton)
        {
            this.userManager = userManager;
            this._config = _config;
            this.signInManager = signInManager;
            this.serviceSingleton = serviceSingleton;
        }

        [HttpPost("registrar")]
        public async Task<ActionResult> registrar([FromBody] UsuarioCreacionDTO nuevoUSer)
        {

            if (!nuevoUSer.Email.Equals(nuevoUSer.EmailConfirmed)) return BadRequest("Los correos no coinciden");
            if (!nuevoUSer.Password.Equals(nuevoUSer.PasswordConfirmed)) return BadRequest("Las contraseñas no coinciden");

            //Crea un nuevo usuario
            var usuario = new IdentityUser {UserName = nuevoUSer.UserName, Email = nuevoUSer.Email, PhoneNumber = nuevoUSer.PhoneNumber};
            var result = await userManager.CreateAsync(usuario, nuevoUSer.Password);


            if (result.Succeeded)
            {
                //var token = await Generate(mapper.Map<loginUsuarioDTO>(nuevoUSer));
                return Ok();
            }

            else { return BadRequest(result.Errors); }
        }

        [HttpPost("login")]
        public async Task<ActionResult> iniciarSesion(loginUsuarioDTO user)
        {
            //Verifica si el usuario ingresado existe
            var result = await signInManager.PasswordSignInAsync(
                user.UserName, user.Password, isPersistent: false, lockoutOnFailure: false);

            // Si el modelo de user es correcto entra al if
            if (result.Succeeded)
            {
                //esto se utiliza para el recorrido de tarjetas por sesion
                serviceSingleton.tarjetasGanadoras = new List<TarjetaGanadorDTO>();

                //genera el token para la sesion actual.
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

        [HttpPost("hacerAdmin")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> post(string username)
        {
            var usuario = await userManager.FindByNameAsync(username);
            await userManager.AddClaimAsync(usuario, new Claim("administrador", "true"));
            return Ok();
        }

    }
}
