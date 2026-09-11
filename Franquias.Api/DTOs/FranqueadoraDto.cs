namespace Franquias.Api.DTOs
{
    public class AtualizarFranqueadoraDto
    {
        public string? Nome { get; set; }
        public string? NomeFantasia { get; set; }
        public string? RazaoSocial { get; set; }
        public string? Cnpj { get; set; }
    }

    public class FranqueadoraDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string NomeFantasia { get; set; } = string.Empty;
        public string RazaoSocial { get; set; } = string.Empty;
        public string Cnpj { get; set; } = string.Empty;
    }
}
