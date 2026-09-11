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
    public class ChamadosController : ControllerBase
    {
        private readonly FranquiasDbContext _db;

        public ChamadosController(FranquiasDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        [Authorize(Roles = "Administrador,Gestor,Operador")]
        public async Task<ActionResult<List<ChamadoDto>>> ObterTodos([FromQuery] int? unidadeId, [FromQuery] StatusChamado? status)
        {
            var query = _db.Chamados.AsQueryable();

            if (unidadeId.HasValue)
            {
                query = query.Where(c => c.UnidadeId == unidadeId.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(c => c.Status == status.Value);
            }

            var chamados = await query
                .Select(c => new ChamadoDto
                {
                    Id = c.Id,
                    Titulo = c.Titulo,
                    Descricao = c.Descricao,
                    Prioridade = c.Prioridade,
                    Status = c.Status,
                    UnidadeId = c.UnidadeId,
                    UsuarioId = c.UsuarioId,
                    DataAbertura = c.DataAbertura,
                    DataFechamento = c.DataFechamento
                })
                .ToListAsync();

            return Ok(chamados);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Administrador,Gestor,Operador")]
        public async Task<ActionResult<ChamadoDto>> ObterPorId(int id)
        {
            var chamado = await _db.Chamados
                .Where(c => c.Id == id)
                .Select(c => new ChamadoDto
                {
                    Id = c.Id,
                    Titulo = c.Titulo,
                    Descricao = c.Descricao,
                    Prioridade = c.Prioridade,
                    Status = c.Status,
                    UnidadeId = c.UnidadeId,
                    UsuarioId = c.UsuarioId,
                    DataAbertura = c.DataAbertura,
                    DataFechamento = c.DataFechamento
                })
                .FirstOrDefaultAsync();

            if (chamado == null)
            {
                return NotFound();
            }

            return Ok(chamado);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador,Gestor,Operador")]
        public async Task<ActionResult<ChamadoDto>> Criar([FromBody] CriarChamadoDto dto)
        {
            var unidadeExiste = await _db.Unidades.AnyAsync(u => u.Id == dto.UnidadeId);
            if (!unidadeExiste)
            {
                return BadRequest("Unidade informada não existe.");
            }

            var usuario = await _db.Usuarios.OrderBy(u => u.Id).FirstOrDefaultAsync();
            if (usuario == null)
            {
                return BadRequest("Cadastre um usuário antes de criar chamados.");
            }

            var chamado = new Chamado
            {
                Titulo = dto.Titulo,
                Descricao = dto.Descricao,
                Prioridade = dto.Prioridade,
                Status = StatusChamado.Aberto,
                UnidadeId = dto.UnidadeId,
                UsuarioId = usuario.Id,
                DataAbertura = System.DateTime.UtcNow
            };

            _db.Chamados.Add(chamado);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(ObterPorId), new { id = chamado.Id }, new ChamadoDto
            {
                Id = chamado.Id,
                Titulo = chamado.Titulo,
                Descricao = chamado.Descricao,
                Prioridade = chamado.Prioridade,
                Status = chamado.Status,
                UnidadeId = chamado.UnidadeId,
                UsuarioId = chamado.UsuarioId,
                DataAbertura = chamado.DataAbertura,
                DataFechamento = chamado.DataFechamento
            });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador,Gestor,Operador")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] AtualizarChamadoDto dto)
        {
            var chamado = await _db.Chamados.FirstOrDefaultAsync(c => c.Id == id);
            if (chamado == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrWhiteSpace(dto.Titulo)) chamado.Titulo = dto.Titulo;
            if (!string.IsNullOrWhiteSpace(dto.Descricao)) chamado.Descricao = dto.Descricao;
            if (dto.Prioridade.HasValue) chamado.Prioridade = dto.Prioridade.Value;
            if (dto.Status.HasValue) chamado.Status = dto.Status.Value;

            if (dto.Status == StatusChamado.Resolvido || dto.Status == StatusChamado.Fechado)
            {
                chamado.DataFechamento ??= System.DateTime.UtcNow;
            }

            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
