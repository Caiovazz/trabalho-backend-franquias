using System;
using System.ComponentModel.DataAnnotations;

namespace Franquias.Api.DTOs
{
    public class CriarChamadoDto
    {
        [Required]
        public string Titulo { get; set; } = string.Empty;

        [Required]
        public string Descricao { get; set; } = string.Empty;

        [Required]
        public PrioridadeChamado Prioridade { get; set; }

        [Required]
        public int UnidadeId { get; set; }
    }

    public class AtualizarChamadoDto
    {
        public string? Titulo { get; set; }
        public string? Descricao { get; set; }
        public PrioridadeChamado? Prioridade { get; set; }
        public StatusChamado? Status { get; set; }
    }

    public class ChamadoDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public PrioridadeChamado Prioridade { get; set; }
        public StatusChamado Status { get; set; }
        public int UnidadeId { get; set; }
        public int UsuarioId { get; set; }
        public DateTime DataAbertura { get; set; }
        public DateTime? DataFechamento { get; set; }
    }
}
