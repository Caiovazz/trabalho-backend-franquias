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
    public class FornecedoresController : ControllerBase
    {
        private readonly FranquiasDbContext _db;

        public FornecedoresController(FranquiasDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        [Authorize(Roles = "Administrador,Gestor,Operador")]
        public async Task<ActionResult<List<FornecedorDto>>> ObterTodos([FromQuery] int? franqueadoraId)
        {
            var query = _db.Fornecedores.AsQueryable();

            var fornecedores = await query
                .Select(f => new FornecedorDto
                {
                    Id = f.Id,
                    Nome = f.Nome,
                    Cnpj = f.Cnpj,
                    Email = f.Email,
                    Telefone = f.Telefone,
                    Ativo = f.Ativo
                })
                .ToListAsync();

            return Ok(fornecedores);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Administrador,Gestor,Operador")]
        public async Task<ActionResult<FornecedorDto>> ObterPorId(int id)
        {
            var fornecedor = await _db.Fornecedores
                .Where(f => f.Id == id)
                .Select(f => new FornecedorDto
                {
                    Id = f.Id,
                    Nome = f.Nome,
                    Cnpj = f.Cnpj,
                    Email = f.Email,
                    Telefone = f.Telefone,
                    Ativo = f.Ativo
                })
                .FirstOrDefaultAsync();

            if (fornecedor == null)
            {
                return NotFound();
            }

            return Ok(fornecedor);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador,Gestor")]
        public async Task<ActionResult<FornecedorDto>> Criar([FromBody] CriarFornecedorDto dto)
        {
            if (await _db.Fornecedores.AnyAsync(f => f.Cnpj == dto.Cnpj))
            {
                return BadRequest("Já existe um fornecedor com este CNPJ.");
            }

            var fornecedor = new Fornecedor
            {
                Nome = dto.Nome,
                Cnpj = dto.Cnpj,
                Email = dto.Email,
                Telefone = dto.Telefone,
                Ativo = true
            };

            _db.Fornecedores.Add(fornecedor);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(ObterPorId), new { id = fornecedor.Id }, new FornecedorDto
            {
                Id = fornecedor.Id,
                Nome = fornecedor.Nome,
                Cnpj = fornecedor.Cnpj,
                Email = fornecedor.Email,
                Telefone = fornecedor.Telefone,
                Ativo = fornecedor.Ativo
            });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador,Gestor")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] AtualizarFornecedorDto dto)
        {
            var fornecedor = await _db.Fornecedores.FirstOrDefaultAsync(f => f.Id == id);
            if (fornecedor == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrWhiteSpace(dto.Nome)) fornecedor.Nome = dto.Nome;
            if (!string.IsNullOrWhiteSpace(dto.Cnpj)) fornecedor.Cnpj = dto.Cnpj;
            if (!string.IsNullOrWhiteSpace(dto.Email)) fornecedor.Email = dto.Email;
            if (!string.IsNullOrWhiteSpace(dto.Telefone)) fornecedor.Telefone = dto.Telefone;
            if (dto.Ativo.HasValue) fornecedor.Ativo = dto.Ativo.Value;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Deletar(int id)
        {
            var fornecedor = await _db.Fornecedores.FirstOrDefaultAsync(f => f.Id == id);
            if (fornecedor == null)
            {
                return NotFound();
            }

            _db.Fornecedores.Remove(fornecedor);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
