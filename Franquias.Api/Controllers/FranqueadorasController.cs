using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Franquias.Api.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Controllers
{
    [ApiController]
    [Route("api/franqueadoras")]
    public class FranqueadorasController : ControllerBase
    {
        private readonly FranquiasDbContext _db;

        public FranqueadorasController(FranquiasDbContext db)
        {
            _db = db;
        }

        [HttpPost("validar")]
        [ProducesResponseType(typeof(CriarFranqueadoraDto), 200)]
        [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
        public ActionResult<CriarFranqueadoraDto> Validar(
            [FromBody] CriarFranqueadoraDto dados)
        {
            return Ok(dados);
        }

        [HttpGet("test-connection")]
        public ActionResult<bool> TestConnection([FromServices] FranquiasDbContext db)
        {
            try
            {
                var conectado = db.Database.CanConnect();
                return Ok(conectado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Administrador,Gestor")]
        public async Task<ActionResult<List<FranqueadoraDto>>> ObterTodos()
        {
            var franqueadoras = await _db.Franqueadoras
                .Select(f => new FranqueadoraDto
                {
                    Id = f.Id,
                    Nome = f.Nome,
                    NomeFantasia = f.NomeFantasia,
                    RazaoSocial = f.RazaoSocial,
                    Cnpj = f.Cnpj
                })
                .ToListAsync();

            return Ok(franqueadoras);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Administrador,Gestor")]
        public async Task<ActionResult<FranqueadoraDto>> ObterPorId(int id)
        {
            var franqueadora = await _db.Franqueadoras
                .Where(f => f.Id == id)
                .Select(f => new FranqueadoraDto
                {
                    Id = f.Id,
                    Nome = f.Nome,
                    NomeFantasia = f.NomeFantasia,
                    RazaoSocial = f.RazaoSocial,
                    Cnpj = f.Cnpj
                })
                .FirstOrDefaultAsync();

            if (franqueadora == null)
            {
                return NotFound();
            }

            return Ok(franqueadora);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<FranqueadoraDto>> Criar([FromBody] CriarFranqueadoraDto dto)
        {
            if (await _db.Franqueadoras.AnyAsync(f => f.Cnpj == dto.Cnpj))
            {
                return BadRequest("Já existe uma franqueadora com este CNPJ.");
            }

            var franqueadora = new Franqueadora
            {
                NomeFantasia = string.IsNullOrWhiteSpace(dto.NomeFantasia) ? dto.Nome : dto.NomeFantasia,
                RazaoSocial = dto.RazaoSocial ?? dto.Nome,
                Cnpj = dto.Cnpj
            };

            _db.Franqueadoras.Add(franqueadora);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(ObterPorId), new { id = franqueadora.Id }, new FranqueadoraDto
            {
                Id = franqueadora.Id,
                Nome = franqueadora.Nome,
                NomeFantasia = franqueadora.NomeFantasia,
                RazaoSocial = franqueadora.RazaoSocial,
                Cnpj = franqueadora.Cnpj
            });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] AtualizarFranqueadoraDto dto)
        {
            var franqueadora = await _db.Franqueadoras.FirstOrDefaultAsync(f => f.Id == id);
            if (franqueadora == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrWhiteSpace(dto.Nome))
                franqueadora.NomeFantasia = dto.Nome;
            if (!string.IsNullOrWhiteSpace(dto.NomeFantasia))
                franqueadora.NomeFantasia = dto.NomeFantasia;
            if (!string.IsNullOrWhiteSpace(dto.RazaoSocial))
                franqueadora.RazaoSocial = dto.RazaoSocial;
            if (!string.IsNullOrWhiteSpace(dto.Cnpj))
                franqueadora.Cnpj = dto.Cnpj;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Deletar(int id)
        {
            var franqueadora = await _db.Franqueadoras.FirstOrDefaultAsync(f => f.Id == id);
            if (franqueadora == null)
            {
                return NotFound();
            }

            _db.Franqueadoras.Remove(franqueadora);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}