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
    public class RoyaltyController : ControllerBase
    {
        private readonly FranquiasDbContext _db;

        public RoyaltyController(FranquiasDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        [Authorize(Roles = "Administrador,Gestor")]
        public async Task<ActionResult<List<RoyaltyConfigDto>>> ObterConfiguracoes()
        {
            var configs = await _db.RoyaltyConfigs
                .Select(r => new RoyaltyConfigDto
                {
                    Id = r.Id,
                    UnidadeId = r.UnidadeId,
                    Percentual = r.Percentual
                })
                .ToListAsync();

            return Ok(configs);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<RoyaltyConfigDto>> Criar([FromBody] CriarRoyaltyConfigDto dto)
        {
            var unidadeExiste = await _db.Unidades.AnyAsync(u => u.Id == dto.UnidadeId);
            if (!unidadeExiste)
            {
                return BadRequest("Unidade informada não existe.");
            }

            var config = new RoyaltyConfig
            {
                UnidadeId = dto.UnidadeId,
                Percentual = dto.Percentual
            };

            _db.RoyaltyConfigs.Add(config);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(ObterConfiguracoes), new { id = config.Id }, new RoyaltyConfigDto
            {
                Id = config.Id,
                UnidadeId = config.UnidadeId,
                Percentual = config.Percentual
            });
        }

        [HttpGet("calcular")]
        [Authorize(Roles = "Administrador,Gestor")]
        public async Task<ActionResult<object>> CalcularRoyalties([FromQuery] int unidadeId, [FromQuery] DateTime inicio, [FromQuery] DateTime fim)
        {
            var unidade = await _db.Unidades
                .FirstOrDefaultAsync(u => u.Id == unidadeId);

            if (unidade == null)
            {
                return BadRequest("Unidade informada não existe.");
            }

            var config = await _db.RoyaltyConfigs
                .Where(r => r.UnidadeId == unidadeId)
                .OrderByDescending(r => r.Id)
                .FirstOrDefaultAsync();

            if (config == null)
            {
                return BadRequest("Não existe configuração de royalties para esta unidade.");
            }

            var faturamento = await _db.Vendas
                .Where(v => v.UnidadeId == unidadeId)
                .Where(v => v.DataVenda >= inicio && v.DataVenda <= fim)
                .SumAsync(v => v.Total);

            var royalties = faturamento * (config.Percentual / 100m);

            return Ok(new
            {
                UnidadeId = unidadeId,
                Percentual = config.Percentual,
                Faturamento = faturamento,
                Royalties = royalties
            });
        }
    }
}
