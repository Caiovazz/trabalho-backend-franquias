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
    public class VendasController : ControllerBase
    {
        private readonly FranquiasDbContext _db;

        public VendasController(FranquiasDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        [Authorize(Roles = "Administrador,Gestor,Operador")]
        public async Task<ActionResult<List<VendaDto>>> ObterTodos([FromQuery] int? unidadeId)
        {
            var query = _db.Vendas.AsQueryable();

            if (unidadeId.HasValue)
            {
                query = query.Where(v => v.UnidadeId == unidadeId.Value);
            }

            var vendas = await query
                .Select(v => new VendaDto
                {
                    Id = v.Id,
                    UnidadeId = v.UnidadeId,
                    UsuarioId = v.UsuarioId,
                    DataVenda = v.DataVenda,
                    Total = v.Total,
                    Itens = v.Itens.Select(i => new ItemVendaDto
                    {
                        ProdutoId = i.ProdutoId,
                        Quantidade = i.Quantidade
                    }).ToList()
                })
                .ToListAsync();

            return Ok(vendas);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador,Gestor,Operador")]
        public async Task<ActionResult<VendaDto>> Criar([FromBody] CriarVendaDto dto)
        {
            var unidade = await _db.Unidades.FirstOrDefaultAsync(u => u.Id == dto.UnidadeId);
            if (unidade == null)
            {
                return NotFound("Unidade não encontrada.");
            }

            if (!unidade.Ativa)
            {
                return BadRequest("Não é permitido vender para uma unidade inativa.");
            }

            if (dto.Itens == null || !dto.Itens.Any())
            {
                return BadRequest("A venda precisa conter itens.");
            }

            var usuario = await _db.Usuarios.OrderBy(u => u.Id).FirstOrDefaultAsync();
            if (usuario == null)
            {
                return BadRequest("Cadastre um usuário antes de registrar vendas.");
            }

            var venda = new Venda
            {
                UnidadeId = dto.UnidadeId,
                UsuarioId = usuario.Id,
                DataVenda = System.DateTime.UtcNow,
                Total = 0
            };

            decimal total = 0;
            var itens = new List<ItemVenda>();

            foreach (var itemDto in dto.Itens)
            {
                var produto = await _db.Produtos.FirstOrDefaultAsync(p => p.Id == itemDto.ProdutoId);
                if (produto == null)
                {
                    return BadRequest($"Produto {itemDto.ProdutoId} não encontrado.");
                }

                if (produto.Situacao != SituacaoProduto.Ativo)
                {
                    return BadRequest($"Produto {produto.Nome} não está ativo.");
                }

                var estoque = await _db.Estoques
                    .FirstOrDefaultAsync(e => e.ProdutoId == itemDto.ProdutoId && e.UnidadeId == dto.UnidadeId);

                if (estoque == null || estoque.Quantidade < itemDto.Quantidade)
                {
                    return BadRequest($"Estoque insuficiente para o produto {produto.Nome}.");
                }

                var subtotal = produto.Preco * itemDto.Quantidade;
                total += subtotal;

                itens.Add(new ItemVenda
                {
                    ProdutoId = itemDto.ProdutoId,
                    Quantidade = itemDto.Quantidade,
                    PrecoUnitario = produto.Preco,
                    Subtotal = subtotal
                });
            }

            venda.Total = total;
            venda.Itens = itens;

            _db.Vendas.Add(venda);
            await _db.SaveChangesAsync();

            foreach (var item in itens)
            {
                var estoque = await _db.Estoques
                    .FirstOrDefaultAsync(e => e.ProdutoId == item.ProdutoId && e.UnidadeId == dto.UnidadeId);

                if (estoque == null)
                {
                    continue;
                }

                estoque.Quantidade -= item.Quantidade;

                _db.MovimentosEstoque.Add(new MovimentoEstoque
                {
                    EstoqueUnidadeId = estoque.Id,
                    Tipo = TipoMovimentoEstoque.Saida,
                    Quantidade = item.Quantidade,
                    Motivo = "Venda",
                    DataMovimento = System.DateTime.UtcNow
                });
            }

            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(ObterTodos), new { unidadeId = dto.UnidadeId }, new VendaDto
            {
                Id = venda.Id,
                UnidadeId = venda.UnidadeId,
                UsuarioId = venda.UsuarioId,
                DataVenda = venda.DataVenda,
                Total = venda.Total,
                Itens = dto.Itens
            });
        }
    }
}
