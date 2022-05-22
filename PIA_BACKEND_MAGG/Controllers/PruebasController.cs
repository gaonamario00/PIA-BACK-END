using Microsoft.AspNetCore.Mvc;
using PIA_BACKEND_MAGG.Filtros;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PIA_BACKEND_MAGG.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PruebasController : ControllerBase
    {
        // Solo se utiliza para la prueba del CORS, respondeCache y filtro de accion
        [HttpGet("CORS")]
        [ResponseCache(Duration = 15)]
        //[ServiceFilter(typeof(FiltroDeAccion))]
        public async Task<ActionResult> CORSTest()
        {
            return Ok("Funciona CORS");
        }

        // Solo se usa para el filtro global
        [HttpGet("Prueba")]
        [ResponseCache(Duration = 15)]
        [ServiceFilter(typeof(FiltroDeException))]
        public async Task<ActionResult> pruebaException()
        {
            new Exception("Exception de prueba");
            return Ok();
        }
    }
}
