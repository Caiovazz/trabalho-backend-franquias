using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RelatoriosController : ControllerBase
    {
        private readonly FranquiasDbContext _db;

        public RelatoriosController(FranquiasDbContext db)
        {
            _db = db;
        }

        [HttpGet("faturamento")]
        [Authorize(Roles = "Administrador,Gestor")]
        public async Task<ActionResult<object>> Faturamento([FromQuery] int? franqueadoraId, [FromQuery] DateTime? inicio, [FromQuery] DateTime? fim)
        {
            var query = _db.Vendas
                .Include(v => v.Unidade)
                .AsQueryable();

            if (franqueadoraId.HasValue)
            {
                query = query.Where(v => v.Unidade != null && v.Unidade.FranqueadoraId == franqueadoraId.Value);
            }

            if (inicio.HasValue)
            {
                query = query.Where(v => v.DataVenda >= inicio.Value);
            }

            if (fim.HasValue)
            {
                query = query.Where(v => v.DataVenda <= fim.Value);
            }

            var faturamento = await query
                .GroupBy(v => v.Unidade != null ? v.Unidade.FranqueadoraId : 0)
                .Select(g => new
                {
                    FranqueadoraId = g.Key,
                    Total = g.Sum(v => v.Total)
                })
                .ToListAsync();

            return Ok(faturamento);
        }

        [HttpGet("produtos-mais-vendidos")]
        [Authorize(Roles = "Administrador,Gestor")]
        public async Task<ActionResult<object>> ProdutosMaisVendidos([FromQuery] int? franqueadoraId)
        {
            var itens = await _db.ItensVenda
                .Include(i => i.Produto)
                .Include(i => i.Venda!)
                .ThenInclude(v => v!.Unidade)
                .ToListAsync();

            if (franqueadoraId.HasValue)
            {
                itens = itens
                    .Where(i => i.Venda?.Unidade?.FranqueadoraId == franqueadoraId.Value)
                    .ToList();
            }

            var result = itens
                .GroupBy(i => new { i.ProdutoId, ProdutoNome = i.Produto?.Nome ?? string.Empty })
                .Select(g => new
                {
                    ProdutoId = g.Key.ProdutoId,
                    ProdutoNome = g.Key.ProdutoNome,
                    QuantidadeVendida = g.Sum(x => x.Quantidade)
                })
                .OrderByDescending(x => x.QuantidadeVendida)
                .ToList();

            return Ok(result);
        }

        [HttpGet("chamados-por-status")]
        [Authorize(Roles = "Administrador,Gestor")]
        public async Task<ActionResult<object>> ChamadosPorStatus([FromQuery] int? franqueadoraId)
        {
            var query = _db.Chamados
                .Include(c => c.Unidade)
                .AsQueryable();

            if (franqueadoraId.HasValue)
            {
                query = query.Where(c => c.Unidade != null && c.Unidade.FranqueadoraId == franqueadoraId.Value);
            }

            var result = await query
                .GroupBy(c => c.Status)
                .Select(g => new
                {
                    Status = g.Key,
                    Quantidade = g.Count()
                })
                .ToListAsync();

            return Ok(result);
        }
    }
}
