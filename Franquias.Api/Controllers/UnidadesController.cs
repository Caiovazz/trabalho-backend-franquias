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
    [Route("api/[controller]")]
    public class UnidadesController : ControllerBase
    {
        private readonly FranquiasDbContext _db;

        public UnidadesController(FranquiasDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        [Authorize(Roles = "Administrador,Gestor,Operador")]
        public async Task<ActionResult<List<UnidadeDto>>> ObterTodos([FromQuery] int? franqueadoraId)
        {
            var query = _db.Unidades.AsQueryable();

            if (franqueadoraId.HasValue)
            {
                query = query.Where(u => u.FranqueadoraId == franqueadoraId.Value);
            }

            var unidades = await query
                .Select(u => new UnidadeDto
                {
                    Id = u.Id,
                    Nome = u.Nome,
                    Cnpj = u.Cnpj,
                    Endereco = u.Endereco,
                    Cidade = u.Cidade,
                    Estado = u.Estado,
                    Telefone = u.Telefone,
                    PercentualRoyalty = u.PercentualRoyalty,
                    Ativa = u.Ativa,
                    DataInicio = u.DataInicio,
                    FranqueadoraId = u.FranqueadoraId
                })
                .ToListAsync();

            return Ok(unidades);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Administrador,Gestor,Operador")]
        public async Task<ActionResult<UnidadeDto>> ObterPorId(int id)
        {
            var unidade = await _db.Unidades
                .Where(u => u.Id == id)
                .Select(u => new UnidadeDto
                {
                    Id = u.Id,
                    Nome = u.Nome,
                    Cnpj = u.Cnpj,
                    Endereco = u.Endereco,
                    Cidade = u.Cidade,
                    Estado = u.Estado,
                    Telefone = u.Telefone,
                    PercentualRoyalty = u.PercentualRoyalty,
                    Ativa = u.Ativa,
                    DataInicio = u.DataInicio,
                    FranqueadoraId = u.FranqueadoraId
                })
                .FirstOrDefaultAsync();

            if (unidade == null)
            {
                return NotFound();
            }

            return Ok(unidade);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador,Gestor")]
        public async Task<ActionResult<UnidadeDto>> Criar([FromBody] CriarUnidadeDto dto)
        {
            var franqueadoraExiste = await _db.Franqueadoras.AnyAsync(f => f.Id == dto.FranqueadoraId);
            if (!franqueadoraExiste)
            {
                return BadRequest("Franqueadora informada não existe.");
            }

            if (await _db.Unidades.AnyAsync(u => u.Cnpj == dto.Cnpj))
            {
                return BadRequest("Já existe uma unidade com este CNPJ.");
            }

            var unidade = new Unidade
            {
                Nome = dto.Nome,
                Cnpj = dto.Cnpj,
                Endereco = dto.Endereco,
                Cidade = dto.Cidade,
                Estado = dto.Estado,
                Telefone = dto.Telefone,
                PercentualRoyalty = dto.PercentualRoyalty,
                Ativa = true,
                DataInicio = DateTime.UtcNow,
                FranqueadoraId = dto.FranqueadoraId
            };

            _db.Unidades.Add(unidade);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(ObterPorId), new { id = unidade.Id }, new UnidadeDto
            {
                Id = unidade.Id,
                Nome = unidade.Nome,
                Cnpj = unidade.Cnpj,
                Endereco = unidade.Endereco,
                Cidade = unidade.Cidade,
                Estado = unidade.Estado,
                Telefone = unidade.Telefone,
                PercentualRoyalty = unidade.PercentualRoyalty,
                Ativa = unidade.Ativa,
                DataInicio = unidade.DataInicio,
                FranqueadoraId = unidade.FranqueadoraId
            });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador,Gestor")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] AtualizarUnidadeDto dto)
        {
            var unidade = await _db.Unidades.FirstOrDefaultAsync(u => u.Id == id);
            if (unidade == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrWhiteSpace(dto.Nome)) unidade.Nome = dto.Nome;
            if (!string.IsNullOrWhiteSpace(dto.Cnpj)) unidade.Cnpj = dto.Cnpj;
            if (!string.IsNullOrWhiteSpace(dto.Endereco)) unidade.Endereco = dto.Endereco;
            if (!string.IsNullOrWhiteSpace(dto.Cidade)) unidade.Cidade = dto.Cidade;
            if (!string.IsNullOrWhiteSpace(dto.Estado)) unidade.Estado = dto.Estado;
            if (!string.IsNullOrWhiteSpace(dto.Telefone)) unidade.Telefone = dto.Telefone;
            if (dto.PercentualRoyalty.HasValue) unidade.PercentualRoyalty = dto.PercentualRoyalty.Value;
            if (dto.Ativa.HasValue) unidade.Ativa = dto.Ativa.Value;
            if (dto.DataInicio.HasValue) unidade.DataInicio = dto.DataInicio.Value;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Deletar(int id)
        {
            var unidade = await _db.Unidades.FirstOrDefaultAsync(u => u.Id == id);
            if (unidade == null)
            {
                return NotFound();
            }

            _db.Unidades.Remove(unidade);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
