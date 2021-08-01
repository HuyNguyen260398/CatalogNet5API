using System.ComponentModel.DataAnnotations;

namespace Catalog.Api.DTOs
{
    public record CreateItemDTO
    {
        [Required]
        public string Name { get; init; }

        [Required]
        [Range(1, 1000)]
        public decimal Price { get; init; }
    }
}