using System.ComponentModel.DataAnnotations;

namespace Franquias.Api.DTOs
{
    public class CriarRoyaltyConfigDto
    {
        [Required]
        public int UnidadeId { get; set; }

        [Range(0, 100)]
        public decimal Percentual { get; set; }
    }

    public class RoyaltyConfigDto
    {
        public int Id { get; set; }
        public int UnidadeId { get; set; }
        public decimal Percentual { get; set; }
    }
}
