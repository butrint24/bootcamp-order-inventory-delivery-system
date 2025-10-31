using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs
{
    public class ShoppingCartDto
    {
        [Required]
        public Dictionary<Guid, int> ItemsAndQuantities { get; set; } = new();

    }
}