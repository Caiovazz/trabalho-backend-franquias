using Franquias.Api.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Franquias.Api.Controllers
{
    [ApiController]
    [Route("api/franqueadoras")]
    public class FranqueadorasController : ControllerBase
    {
        [HttpPost("validar")]
        [ProducesResponseType(typeof(CriarFranqueadoraDto), 200)]
        [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
        public ActionResult<CriarFranqueadoraDto> Validar(
            [FromBody] CriarFranqueadoraDto dados)
        {
            return Ok(dados);
        }
    }
}