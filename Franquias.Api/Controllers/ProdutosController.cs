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
    public class ProdutosController : ControllerBase
    {
        private readonly FranquiasDbContext _db;

        public ProdutosController(FranquiasDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        [Authorize(Roles = "Administrador,Gestor,Operador")]
        public async Task<ActionResult<List<ProdutoDto>>> ObterTodos([FromQuery] int? franqueadoraId)
        {
            var query = _db.Produtos.AsQueryable();

            var produtos = await query
                .Select(p => new ProdutoDto
                {
                    Id = p.Id,
                    Nome = p.Nome,
                    Descricao = p.Descricao,
                    Preco = p.Preco,
                    EstoqueMinimo = p.EstoqueMinimo,
                    CategoriaId = p.CategoriaId,
                    FornecedorId = p.FornecedorId,
                    Ativo = p.Ativo
                })
                .ToListAsync();

            return Ok(produtos);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Administrador,Gestor,Operador")]
        public async Task<ActionResult<ProdutoDto>> ObterPorId(int id)
        {
            var produto = await _db.Produtos
                .Where(p => p.Id == id)
                .Select(p => new ProdutoDto
                {
                    Id = p.Id,
                    Nome = p.Nome,
                    Descricao = p.Descricao,
                    Preco = p.Preco,
                    EstoqueMinimo = p.EstoqueMinimo,
                    CategoriaId = p.CategoriaId,
                    FornecedorId = p.FornecedorId,
                    Ativo = p.Ativo
                })
                .FirstOrDefaultAsync();

            if (produto == null)
            {
                return NotFound();
            }

            return Ok(produto);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador,Gestor")]
        public async Task<ActionResult<ProdutoDto>> Criar([FromBody] CriarProdutoDto dto)
        {
            var categoriaExiste = await _db.Categorias.AnyAsync(c => c.Id == dto.CategoriaId);
            if (!categoriaExiste)
            {
                return BadRequest("Categoria informada não existe.");
            }

            if (dto.FornecedorId.HasValue)
            {
                var fornecedorExiste = await _db.Fornecedores.AnyAsync(f => f.Id == dto.FornecedorId.Value);
                if (!fornecedorExiste)
                {
                    return BadRequest("Fornecedor informado não existe.");
                }
            }

            var produto = new Produto
            {
                Nome = dto.Nome,
                Descricao = dto.Descricao,
                Preco = dto.Preco,
                EstoqueMinimo = dto.EstoqueMinimo,
                CategoriaId = dto.CategoriaId,
                FornecedorId = dto.FornecedorId,
                Ativo = dto.Ativo
            };

            _db.Produtos.Add(produto);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(ObterPorId), new { id = produto.Id }, new ProdutoDto
            {
                Id = produto.Id,
                Nome = produto.Nome,
                Descricao = produto.Descricao,
                Preco = produto.Preco,
                EstoqueMinimo = produto.EstoqueMinimo,
                CategoriaId = produto.CategoriaId,
                FornecedorId = produto.FornecedorId,
                Ativo = produto.Ativo
            });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador,Gestor")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] AtualizarProdutoDto dto)
        {
            var produto = await _db.Produtos.FirstOrDefaultAsync(p => p.Id == id);
            if (produto == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrWhiteSpace(dto.Nome)) produto.Nome = dto.Nome;
            if (!string.IsNullOrWhiteSpace(dto.Descricao)) produto.Descricao = dto.Descricao;
            if (dto.Preco.HasValue) produto.Preco = dto.Preco.Value;
            if (dto.EstoqueMinimo.HasValue) produto.EstoqueMinimo = dto.EstoqueMinimo.Value;
            if (dto.CategoriaId.HasValue) produto.CategoriaId = dto.CategoriaId.Value;
            if (dto.FornecedorId.HasValue) produto.FornecedorId = dto.FornecedorId.Value;
            if (dto.Ativo.HasValue) produto.Ativo = dto.Ativo.Value;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Deletar(int id)
        {
            var produto = await _db.Produtos.FirstOrDefaultAsync(p => p.Id == id);
            if (produto == null)
            {
                return NotFound();
            }

            _db.Produtos.Remove(produto);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
