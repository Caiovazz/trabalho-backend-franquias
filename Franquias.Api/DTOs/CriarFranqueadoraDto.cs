using System.ComponentModel.DataAnnotations;

namespace Franquias.Api.DTOs
{
    public class CriarFranqueadoraDto
    {
        public string Nome { get; set; } = string.Empty;

        public string? NomeFantasia { get; set; }

        public string? RazaoSocial { get; set; }

        [Required(ErrorMessage = "Informe o CNPJ.")]
        public string Cnpj { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe o e-mail.")]
        [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
        public string Email { get; set; } = string.Empty;
    }
}